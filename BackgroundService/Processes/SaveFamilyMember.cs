using BackgroundService.Models;
using Intersoft.CISSA.DataAccessLayer.Model.Context;
using Intersoft.CISSA.DataAccessLayer.Model.Documents;
using Intersoft.CISSA.DataAccessLayer.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace BackgroundService.Processes
{
    public class SaveFamilyMember
    {
        private WorkflowContext context;
        public string CacheKey;
        public TaskInfo TaskInfo;
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();
        public SaveFamilyMember(WorkflowContext context, string cacheKey, Dictionary<string, object> parameters)
        {
            this.context = context;
            this.TaskInfo = Caching.Get<TaskInfo>(CacheKey) ?? new TaskInfo();
            this.Parameters = parameters;

            InitDocument();

            TaskInfo.SetState("шаг 3 из 4. Сохранен, привязка к заявлению...");
            Caching.Set(CacheKey, TaskInfo);

            Execute();

            TaskInfo.SetState("шаг 4 из 4. Привязан, обновление информации о составе семьи...");
            Caching.Set(CacheKey, TaskInfo);
            
            FamilyRecalc();
            
            TaskInfo.SetState("Обновлена. Процесс завершен.");
            Caching.Set(CacheKey, TaskInfo);
        }

        void InitDocument()
        {
            var fMemberDefId = new Guid("{959762D8-FEB6-4247-986D-7ECE63EED1AD}");
            var doc = context.Documents.New(fMemberDefId);
            foreach (var p in Parameters)
            {
                if (p.Value != null) doc[p.Key] = p.Value;
            }
            context.CurrentDocument = doc;
        }

        void Execute()
        {
            context.SuccessFlag = false;

            //var docRepo = context.Documents;
            Doc app = (Doc)context["Application_State"];

            if (app != null)
            {
                var docRepo = context.Documents;
                docRepo.AddDocToList(context.CurrentDocument.Id, app, "Family_Member");
                
                TaskInfo.SetState("Added to member-list");
                Caching.Set(CacheKey, TaskInfo);
                
                context.SuccessFlag = true;
                context["AppStateForFamilyRecalc"] = app;
            }
            else
                throw new ApplicationException("Заявление не определено!");
        }

        void FamilyRecalc()
        {
            if (context["AppStateForFamilyRecalc"] == null)
                throw new ApplicationException("Документ для подсчета кол-ва членов домохозяйства не передан!");
            var appState = (Doc)context["AppStateForFamilyRecalc"];
            //reset values
            appState["Children_under_16"] = null;
            appState["children_disabilities"] = null;
            appState["group_1"] = null;
            appState["Older_than_65"] = null;
            var docRepo = context.Documents;
            var childDisabilityCatEnumId = new Guid("{1BAEA3E7-7BDA-477F-A077-CD3ACF6A63C5}"); //{1BAEA3E7-7BDA-477F-A077-CD3ACF6A63C5}
            var app = docRepo.LoadById((Guid)appState["Application"]);
            //Calc for Applicant              
            CalcValues((Guid?)appState["DisabilityGroupe"] ?? Guid.Empty, docRepo.LoadById((Guid)app["Person"]), appState, true);
            //Calc for family members
            int memCount = 0;
            int c;
            foreach (var memId in docRepo.DocAttrList(out c, appState, "Family_Member", 0, 0))
            {
                var mem = docRepo.LoadById(memId);
                var memCatId = (Guid?)mem["Disability"] ?? Guid.Empty;
                CalcValues(memCatId == childDisabilityCatEnumId ? childDisabilityCatEnumId : (Guid?)mem["DisablilityGroupe"] ?? Guid.Empty, docRepo.LoadById((Guid)mem["Person"]), appState, false);
                memCount++;
            }

            appState["all"] = memCount + 1;
            docRepo.Save(appState);
        }

        private void CalcValues(Guid disabilityId, Doc person, Doc appState, bool isApplicant)
        {
            var noDisabilityEnumId = new Guid("{C3EB23D0-9806-4C9B-B087-5987B763086D}");//нет группы
            var group1EnumId = new Guid("{CC195B10-995A-44DB-A852-F519E0AD1BED}");//new Guid("{11CA0BDE-5F53-4574-99BD-1B31C23B2662}");//I group
            var childDisabilityCatEnumId = new Guid("{1BAEA3E7-7BDA-477F-A077-CD3ACF6A63C5}");
            var birthDate = (DateTime)person["Date_of_Birth"];
            var year = DateTime.Today.Year;
            var pYear = birthDate.Year;
            var age = year - pYear;
            if (isApplicant) appState["Ages"] = age;
            if (age <= 16)
            {
                appState["Children_under_16"] = ((int?)appState["Children_under_16"] ?? 0) + 1;
                if (/*disabilityId != noDisabilityEnumId && */disabilityId == childDisabilityCatEnumId/*Guid.Empty*/)
                    appState["children_disabilities"] = ((int?)appState["children_disabilities"] ?? 0) + 1;
            }
            else
            {
                if (disabilityId == group1EnumId/* && age >= 18*/)//disabilityId != noDisabilityEnumId && disabilityId != Guid.Empty)
                    appState["group_1"] = ((int?)appState["group_1"] ?? 0) + 1;
                else if (/*age < 18 && (disabilityId != noDisabilityEnumId && */disabilityId == childDisabilityCatEnumId/*Guid.Empty)*/)
                    appState["children_disabilities"] = ((int?)appState["children_disabilities"] ?? 0) + 1;
                if (age >= 65)
                    appState["Older_than_65"] = ((int?)appState["Older_than_65"] ?? 0) + 1;
            }
        }
    }
}