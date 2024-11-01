using Newtonsoft.Json;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Domain
{
    public class MainEntityClass : BaseEntityClass
    {
        public IEnumerable<BaseIndicatorModel> GetIndicators(string file)
        {
            var json = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + file);
            var indicators = JsonConvert.DeserializeObject<IEnumerable<BaseIndicatorModel>>(json) as List<BaseIndicatorModel>;
            indicators[0].Value = indicators[0].Value * 100;
            indicators[1].Value = indicators[1].Value * 100;
            indicators[2].Value = indicators[2].Value * 100;
            indicators[3].Value = indicators[3].Value * 100;
            indicators[4].Value = indicators[4].Value * 100;
            return indicators;
        }

        public IndicatorsClass GetResponsiblePersonProperties()
        {
            List<ResponsiblePersonsProperties> personsProperties = new List<ResponsiblePersonsProperties>();
            foreach (var item in dataBase.ResponsiblePersonsProperties.AsEnumerable())
            {
                personsProperties.Add(item);
            }
            List<BaseIndicatorModel> indicators = GetIndicators("IndicatorConstants.json") as List<BaseIndicatorModel>;
            IndicatorsClass indicatorsClass = new IndicatorsClass() { BaseIndicatorModel = indicators, ResponsiblePersonsProperties = personsProperties };
            return indicatorsClass;
        }

        public IEnumerable<Clients> GetClients(string condition, string key)
        {
            IEnumerable<Clients> clients = dataBase.Clients;
            if (!String.IsNullOrEmpty(key))
            {
                clients = clients.Where(s => s.Name.ToUpper().Contains(key.ToUpper()));
            }
            switch (condition)
            {
                case "1":
                    clients = dataBase.Clients.Where(client => client.Name.Contains(key));
                    break;
                case "2":
                    clients = dataBase.Clients.Where(client => client.UNP.Contains(key));
                    break;

                default:
                    clients = dataBase.Clients.OrderBy(client => client.Id);
                    break;
            }
            return clients;
        }
        public Products GetProduct(int ProductId)
        {
            var products = dataBase.Products.Find(ProductId);
            return products;
        }
        public IEnumerable<Accessories> Materials(int ProductId)
        {
            var products = GetProduct(ProductId);
            IEnumerable<Materials> materials = dataBase.Materials;
            IEnumerable<Accessories> accessories = dataBase.Accessories;
            return accessories =
                from accessory in accessories
                join material in materials on accessory.Id equals material.AccessoryId
                where material.ProductId == ProductId
                select new Accessories() { Id = accessory.Id, Name = accessory.Name, MeasureUnit = accessory.MeasureUnit, 
                    isDeleted = accessory.isDeleted, Value = accessory.Value };
        }

        public IEnumerable<Accessories> GetMaterials(string key, int ProductId)
        {
            IEnumerable<Accessories> accessories = dataBase.Accessories;
            IEnumerable<Materials> materials = dataBase.Materials;
            if (!String.IsNullOrEmpty(key))
            {
                accessories = accessories.Where(s => s.Name.ToUpper().Contains(key.ToUpper()));
            }
            Products products = GetProduct(ProductId);
            return key == null ? accessories =
                from accessory in accessories
                join material in materials on accessory.Id equals material.AccessoryId
                where material.ProductId == ProductId
                select new Accessories() { Id = accessory.Id, Name = accessory.Name, Value = accessory.Value } : accessories =
                from accessory in accessories
                where accessory.Name.ToUpper().Contains(key.ToUpper())
                join material in materials on accessory.Id equals material.AccessoryId
                where material.ProductId == ProductId
                select new Accessories() { Id = accessory.Id, Name = accessory.Name, Value = accessory.Value };
        }

        public IEnumerable<Jobs> GetJobs(string key)
        {
            IEnumerable<Jobs> jobs = dataBase.Jobs;
            if (!String.IsNullOrEmpty(key))
            {
                jobs = jobs.Where(s => s.Name.ToUpper().Contains(key.ToUpper()));
            }

            return key == null ? jobs : jobs = dataBase.Jobs.Where(job => job.Name.Contains(key));
        }

        public IEnumerable<Accessories> GetAccessories(string key)
        {
            IEnumerable<Accessories> accessories = dataBase.Accessories;
            accessories = accessories.OrderBy(ac => ac.Name);
            if (!String.IsNullOrEmpty(key))
            {
                accessories = accessories.Where(s => s.Name.ToUpper().Contains(key.ToUpper())).ToList().OrderBy(ac => ac.Name);
            }
            return key == null ? accessories : accessories = dataBase.Accessories.Where(accessor => accessor.Name.Contains(key)).OrderBy(ac => ac.Name);
        }

        public IEnumerable<Repairs> Repair(string condition, string key, string dateFrom, string toDate,
            string selectedSort, string sortDirection)
        {
            if (condition == null || key == null)
            {
                IEnumerable<Repairs> repair = dataBase.Repairs;
                repair =
                    from rep in repair
                    where DateTime.Now.Date.Year <= rep.DateToRepair.Date.Year &&
                    rep.DateToRepair.Date.Year <= DateTime.Now.Date.AddYears(1).Year
                    select rep;
                repair = repair.Where(r => r.Deleted == false);
                return repair;
            }
            else
                return GetRepairsV2(condition, key, dateFrom, toDate, selectedSort, sortDirection, false);
        }

        public IEnumerable<Repairs> DeletedRepairs(string condition, string key, string dateFrom, string toDate,
            string selectedSort, string sortDirection)
        {
            if (condition == null || key == null)
            {
                IEnumerable<Repairs> repair = dataBase.Repairs;
                repair =
                    from rep in repair
                    where DateTime.Now.Date.Year <= rep.DateToRepair.Date.Year &&
                    rep.DateToRepair.Date.Year <= DateTime.Now.Date.AddYears(1).Year
                    select rep;
                repair = repair.Where(r => r.Deleted == true);
                return repair;
            }
            else
                return GetRepairsV2(condition, key, dateFrom, toDate, selectedSort, sortDirection, true);
        }

        public IEnumerable<Repairs> GetRepairsV2(string condition, string key, string dateFrom,
            string toDate, string selectedSort, string sortDirection, bool IsDeleted)
        {
            var currentRepairs = new List<Repairs>();
            IEnumerable<Repairs> repairs = dataBase.Repairs;
            if ((dateFrom != "" && dateFrom != null) && (toDate != null && toDate != ""))
            {
                repairs =
                    from rep in repairs
                    where Convert.ToDateTime(dateFrom) <= rep.DateToRepair.Date &&
                    rep.DateToRepair.Date <= Convert.ToDateTime(toDate)
                    select rep;
            }
            else if (dateFrom != "" && dateFrom != null)
            {
                repairs =
                    from rep in repairs
                    where Convert.ToDateTime(dateFrom) <= rep.DateToRepair.Date
                    select rep;
            }
            else if (dateFrom == "" && dateFrom == null)
            {
                repairs =
                    from rep in repairs
                    where rep.DateToRepair.Date <= Convert.ToDateTime(toDate)
                    select rep;
            }
            else
            {
                repairs =
                    from rep in repairs
                    where DateTime.Now.Date.Year <= rep.DateToRepair.Date.Year &&
                    rep.DateToRepair.Date.Year <= DateTime.Now.Date.AddYears(1).Year
                    select rep;
            }
            switch (selectedSort)
            {
                //заказчик
                case "1":
                    currentRepairs = SortByParams(repairs, 1, Convert.ToBoolean(sortDirection)).ToList();
                    break;
                //Изделию
                case "2":
                    currentRepairs = SortByParams(repairs, 2, Convert.ToBoolean(sortDirection)).ToList();
                    break;
                //Заводскому №
                case "3":
                    currentRepairs = SortByParams(repairs, 3, Convert.ToBoolean(sortDirection)).ToList();
                    break;
                //Номеру RTU
                case "4":
                    currentRepairs = SortByParams(repairs, 4, Convert.ToBoolean(sortDirection)).ToList();
                    break;
                case "5":
                    currentRepairs = SortByParams(repairs, 5, Convert.ToBoolean(sortDirection)).ToList();
                    break;
                case "6":
                    currentRepairs = SortByParams(repairs, 6, Convert.ToBoolean(sortDirection)).ToList();
                    break;
            }
            if(condition != null && key != null)
            {
                currentRepairs = repairs.ToList();
                return SearchCondition(condition, key, currentRepairs, IsDeleted);
            }
            else
                return currentRepairs;
        }
        private IEnumerable<Repairs> SortByParams(IEnumerable<Repairs> repairs, int param, bool sortDirection)
        {
            switch (param)
            {
                //заказчик
                case 1:
                    repairs = repairs.OrderBy(r => r.ClientId).ToList();
                    break;
                //Изделию
                case 2:
                    repairs = repairs.OrderBy(r => r.ProductId).ToList();
                    break;
                //Заводскому №
                case 3:
                    repairs = repairs.OrderBy(r => r.FactoryNumber).ToList();
                    break;
                //Номеру RTU
                case 4:
                    repairs = repairs.OrderBy(r => r.RTUNumber).ToList();
                    break;
                case 5:
                    repairs = repairs.OrderBy(r => r.DateToRepair).ToList();
                    break;
                case 6:
                    repairs = repairs.OrderBy(r => r.Status).ToList();
                    break;
            }
            if (sortDirection)
                return repairs;
            else
                return repairs.Reverse();

        }
        public IEnumerable<Repairs> SearchCondition(string condition, string key, IEnumerable<Repairs> repair, bool IsDeleted)
        {
            switch (condition)
            {
                case "1":
                    IEnumerable<Clients> clients = GetClients(condition, key);
                    repair =
                        from rep in repair
                        join cl in clients on rep.ClientId equals cl.Id
                        where cl.Name.ToUpper().Contains(key.ToUpper())
                        select rep;
                    break;
                case "2":
                    IEnumerable<Products> products = dataBase.Products.Where(prod => prod.Name.Contains(key));
                    repair =
                        from rep in repair
                        join pr in products on rep.ProductId equals pr.Id
                        where pr.Name.ToUpper().Contains(key.ToUpper())
                        select rep;
                    break;
                case "3":
                    repair =
                        from rep in repair
                        where rep.FactoryNumber.Contains(key)
                        select rep;
                    break;
                case "4":
                    repair =
                        from rep in repair
                        where rep.RTUNumber.Contains(key)
                        select rep;
                    break;
                default:
                    repair =
                        from rep in repair
                        where DateTime.Now.Year <= rep.DateToRepair.Date.Year &&
                        rep.DateToRepair.Date.Year <= DateTime.Now.Date.AddYears(1).Year
                        select rep;
                    break;
            }
            repair = repair.Where(r => r.Deleted == IsDeleted);
            return repair;
        }
    }
}