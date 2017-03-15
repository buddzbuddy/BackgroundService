using BackgroundService.Models;
using BackgroundService.Processes;
using Intersoft.CISSA.BizService.Utils;
using Intersoft.CISSA.DataAccessLayer.Core;
using Intersoft.CISSA.DataAccessLayer.Model.Context;
using Intersoft.CISSA.DataAccessLayer.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class Service : IService
    {
        static IAppServiceProvider provider;
        static IAppServiceProviderFactory providerFactory;
        static IMultiDataContext dataContext;
        static IDataContextFactory dataContextFactory;
        
        public Service()
        {
            if (dataContextFactory == null)
            {
                dataContextFactory = DataContextFactoryProvider.GetFactory();

                if (dataContext == null)
                {
                    dataContext = dataContextFactory.CreateMultiDc("DataContexts");
                    BaseServiceFactory.CreateBaseServiceFactories();
                    if (providerFactory == null)
                    {
                        providerFactory = AppServiceProviderFactoryProvider.GetFactory();
                        if (provider == null)
                        {
                            provider = providerFactory.Create(dataContext);
                        }
                    }
                }
            }
        }
        public string GetData(int value)
        {
            var res = "";
            try
            {
                var processId = new Guid("{BAB5331D-A3F8-4748-B17F-F282B2D623BF}");
                var userId = new Guid("{2D6819C9-DB76-43FC-8D9F-EC940539B014}");

                var context = new WorkflowContext(new WorkflowContextData(processId, userId), provider);
                var ui = context.GetUserInfo();

                res = ui.OrganizationName;
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                res = e.Message;
            }
            return res;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public async void StartProcess(string name)
        {
            //Func<Task<object>> valueFactory = () => Task.FromResult((object)(new TestProcess(name)));
            //await _cache.AddOrGetExisting(name, valueFactory);
            
            await Task.Factory.StartNew(() =>
            {
                new TestProcess(name);
            });
        }

        public string GetTaskState(string name)
        {
            var state = "task of \"" + name + "\" not found";
            
            var stateObj = Caching.Get<TaskInfo>(name);

            if (stateObj != null) state = stateObj.CurrentState;

            return state;
            /*var val = Caching.Get<int>(name);//CacheManager.Cache[name];
            return val.ToString();*/
        }

        public async void SaveFamilyMember(Guid userId, Guid Person, Guid? Family_Membership,
            Guid? Disability, Guid Application_State, Guid? DisabilityType, decimal? Family_MemberKON, Guid? DisablilityGroupe, Guid? employment, string DeadInfoFamMem)
        {
            await Task.Factory.StartNew(() =>
            {
                var cacheKey = string.Format("{0}__{1}", userId, Guid.NewGuid());
                var taskInfo = Caching.Get<TaskInfo>(cacheKey) ?? new TaskInfo();
                taskInfo.SetState("шаг 1 из 4. Запуск, подключение(контекст)...");
                Caching.Set(cacheKey, taskInfo);
                var context = Caching.Get<WorkflowContext>(userId.ToString());
                var fromCache = true;
                if (context == null)
                {
                    context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), provider);
                    fromCache = false;
                }
                taskInfo.SetState("шаг 2 из 4. Подключен" + (fromCache ? " \"из кэша\"" : "") + ", инициализация и сохранение...");
                Caching.Set(cacheKey, taskInfo);

                Caching.Set(cacheKey, new SaveFamilyMember(context, cacheKey, new Dictionary<string, object>
                {
                    {"Person", Person},
                    {"Family_Membership", Family_Membership},
                    {"Disability", Disability},
                    {"Application_State", Application_State},
                    {"DisabilityType", DisabilityType},
                    {"Family_MemberKON", Family_MemberKON},
                    {"DisablilityGroupe", DisablilityGroupe},
                    {"employment", employment},
                    {"DeadInfoFamMem", DeadInfoFamMem}
                }));
            });
        }
    }
}
