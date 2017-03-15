using BackgroundServiceClient.ServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundServiceClient
{
    public static class OperationHelper
    {
        public static void SaveFamilyMember(Guid userId, Guid Person, Guid? Family_Membership,
            Guid? Disability, Guid Application_State, Guid? DisabilityType, decimal? Family_MemberKON, Guid? DisablilityGroupe, Guid? employment, string DeadInfoFamMem, out string errorMessage)
        {
            errorMessage = "";
            string processName = "название не указано";
            using (var client = new ServiceClient())
            {
                try
                {
                    client.Open();

                    processName = "Сохранение члена д/х";
                    client.SaveFamilyMember(userId, Person, Family_Membership, Disability, Application_State, DisabilityType, Family_MemberKON, DisablilityGroupe, employment, DeadInfoFamMem);
                }
                catch (Exception e)
                {
                    client.Abort();

                    while (e.InnerException != null) e = e.InnerException;
                    errorMessage = "Ошибка при вызове асинхронного процесса \"" + processName + "\". Текст ошибки: " + e.Message + "; stacktrace: " + e.StackTrace;
                }
                finally
                {
                    if (client.State != System.ServiceModel.CommunicationState.Closed)
                        client.Close();
                }
            }
        }
    }
}
