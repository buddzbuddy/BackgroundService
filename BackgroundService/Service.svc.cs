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
        public string GetData(int value)
        {
            var res = "";
            var val = CacheManager.Cache["value"];
            if (val == null)
            {
                CacheManager.CacheData("value", value);
                res = string.Format("[Origin] You entered: {0}", value);
            }
            else
            {
                res = string.Format("[Cache] You entered: {0}", value);
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


        public async void StartThreadProcess(string name)
        {
            await Task.Factory.StartNew(() =>
            {
                Thread.Sleep(10000);
                Caching.Set<int>(name, 1);
                //CacheManager.CacheData(name, 1);
                Thread.Sleep(10000);
                Caching.Set<int>(name, 2);
                /*var val = CacheManager.Cache[name];
                if (val != null)
                {
                    val = (int)val + 1;
                    Caching.GetObjectFromCache<int>(name, 10, () => val);
                    CacheManager.CacheData(name, val);
                }
                else
                    val = 0;*/
                Thread.Sleep(10000);
                Caching.Set<int>(name, 3);
                /*val = CacheManager.Cache[name];
                if (val != null)
                {
                    val = (int)val + 1;
                    CacheManager.CacheData(name, val);
                }
                else
                    val = 0;*/
                Thread.Sleep(10000);
                Caching.Set<int>(name, 4);
                /*val = CacheManager.Cache[name];
                if (val != null)
                {
                    val = (int)val + 1;
                    CacheManager.CacheData(name, val);
                }
                else
                    val = 0;*/
                Thread.Sleep(10000);
                Caching.Set<int>(name, 5);
                /*val = CacheManager.Cache[name];
                if (val != null)
                {
                    val = (int)val + 1;
                    CacheManager.CacheData(name, val);
                }
                else
                    val = 0;*/
            });
        }

        public string GetValue(string name)
        {
            var val = Caching.Get<int>(name);//CacheManager.Cache[name];
            return val.ToString();
        }
    }
}
