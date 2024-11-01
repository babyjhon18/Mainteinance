using EasyDox;
using Morpher.WebService.V2;
using Newtonsoft.Json;
using Project.Models;
using Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Project.Domain
{
    public class RepairEntityClass : BaseEntityClass
    {
        public bool IsExistedRepair(string factoryNumber, string dateToRepair)
        {
            var existedRepair = dataBase.Repairs.ToList()
                .Where(r => r.FactoryNumber == factoryNumber.ToString() &&
                r.DateToRepair == Convert.ToDateTime(dateToRepair))
                .Select(r => r.Id);
            if (!existedRepair.Any())
                return true;
            else
                return false;
        }
        public RepairClass GetRepair(int id)
        {
            Repairs repairs = dataBase.Repairs.Find(id);

            RepairClass repair = new RepairClass
            {
                Repairs = repairs,
            };
            return repair;
        }

        public void Restore(int id)
        {
            Repairs repairs = dataBase.Repairs.Find(id);
            repairs.Deleted = false;
            dataBase.Entry(repairs).State = EntityState.Modified;
            dataBase.SaveChanges();
        }

        public Dictionary<string, string> Create(Dictionary<string, string> collection)
        {
            var filename = "";
            var engine = new Engine();
            var fieldValues = new Dictionary<string, string>();
            var clientid = 0;
            var dateToRepair = collection["DateToRepair"].ToString();
            if (collection["NewClient"].ToString() != "")
            {
                Clients client = new Clients
                {
                    Name = collection["NewClient"].ToString(),
                    ResponsiblePerson = collection["NewClient"].ToString()
                };
                dataBase.Clients.Add(client);
                dataBase.SaveChanges();
                //client = dataBase.Clients.Where(c => c.Name == collection["NewClient"].ToString());
                clientid = client.Id;
            }
            Repairs repairs = new Repairs
            {
                ClientId = collection["NewClient"].ToString() == "" ? Convert.ToInt32(collection["Client"]) : clientid,
                ProductId = Convert.ToInt32(collection["Product"]),
                FactoryNumber = collection["FactoryNumber"].ToString(),
                RTUNumber = collection["RTUNumber"].ToString(),
                DateToRepair = Convert.ToDateTime(dateToRepair),
                Defect = collection["Defect"].ToString(),
                Set = collection["Set"].ToString(),
                Status = "accepted",
                ContactPerson = collection["ContactPerson"].ToString(),
                Note = collection["Note"],
            };
            var existedRepair = dataBase.Repairs.ToList()
                .Where(r => r.FactoryNumber == collection["FactoryNumber"].ToString() &&
                r.DateToRepair == Convert.ToDateTime(dateToRepair))
                .Select(r => r.Id);
            if (!existedRepair.Any())
            {
                dataBase.Repairs.Add(repairs);
                dataBase.SaveChanges();
                var lastRepair = dataBase.Repairs.ToList().LastOrDefault();
                Clients clients = dataBase.Clients.Find(Convert.ToInt32(collection["Client"]));
                var url = AppDomain.CurrentDomain.BaseDirectory + "App_Data/";
                var product = dataBase.Products.Where(p => p.Id == repairs.ProductId).Select(p => p.Name).FirstOrDefault();
                var month = "";
                switch (repairs.DateToRepair.Month.ToString())
                {
                    case "1":
                        month = "января";
                        break;
                    case "2":
                        month = "февраля";
                        break;
                    case "3":
                        month = "марта";
                        break;
                    case "4":
                        month = "апреля";
                        break;
                    case "5":
                        month = "мая";
                        break;
                    case "6":
                        month = "июня";
                        break;
                    case "7":
                        month = "июля";
                        break;
                    case "8":
                        month = "августа";
                        break;
                    case "9":
                        month = "сентября";
                        break;
                    case "10":
                        month = "октября";
                        break;
                    case "11":
                        month = "ноября";
                        break;
                    case "12":
                        month = "декабря";
                        break;
                }
                var number = "+375(00) 000-00-00";
                if (collection["NewClient"].ToString() == "")
                {
                    if (clients.ContactNumber != null)
                    {
                        number = clients.ContactNumber.ToString();
                    }
                    fieldValues = new Dictionary<string, string> {
                {"Заказчик", clients.Name ?? " "},
                {"Ответственное лицо",  clients.ResponsiblePerson == null ? " " :  clients.ResponsiblePerson},
                {"Телефон",  number},
                {"Изделие", product == null? " " :  product.ToString()},
                {"Номер", repairs.FactoryNumber == null? " " :  repairs.FactoryNumber},
                {"RTU", repairs.RTUNumber == null? " " :  repairs.RTUNumber},
                {"Комплектность", repairs.Set == null? " " :  repairs.Set},
                {"Неисправность", repairs.Defect == null? " " :  repairs.Defect},
                {"ДД", repairs.DateToRepair.Day.ToString()  == null? " " : repairs.DateToRepair.Day.ToString() },
                {"месяц", month },
                {"ГГГГ", repairs.DateToRepair.Year.ToString()  == null? " " : repairs.DateToRepair.Year.ToString() },
                };
                    filename = clients.Name + repairs.DateToRepair.ToShortDateString() + ".docx";
                    engine.Merge(url + "template.docx", fieldValues, url + filename);
                    return new Dictionary<string, string> {
                    { "File", filename },
                    { "LastRepairID", lastRepair.Id.ToString() },
                };
                }
                else
                {
                    var client = collection["NewClient"].ToString();
                    fieldValues = new Dictionary<string, string> {
                {"Заказчик", client},
                {"Ответственное лицо",  client},
                {"Изделие", product == null? " " :  product.ToString()},
                {"Номер", number},
                {"RTU", repairs.RTUNumber == null? " " :  repairs.RTUNumber},
                {"Комплектность", repairs.Set == null? " " :  repairs.Set},
                {"Неисправность", repairs.Defect == null? " " :  repairs.Defect},
                {"ДД", repairs.DateToRepair.Day.ToString()  == null? " " : repairs.DateToRepair.Day.ToString() },
                {"месяц", month },
                {"ГГГГ", repairs.DateToRepair.Year.ToString()  == null? " " : repairs.DateToRepair.Year.ToString() },
                };
                    filename = client + repairs.DateToRepair.ToShortDateString() + ".docx";
                    engine.Merge(url + "template.docx", fieldValues, url + filename);
                    return new Dictionary<string, string> {
                    { "File", filename },
                    { "LastRepairID", lastRepair.Id.ToString() },
                };
                }   
            }
            else
            {
                return new Dictionary<string, string> {
                    { "File", "" },
                    { "LastRepairID", "-1" },
                };
            }
        }

        public String CreateTemplate(Dictionary<string, string> collection)
        {
            var filename = "";
            var engine = new Engine();
            var fieldValues = new Dictionary<string, string>();
            var clientid = 0;
            var dateToRepair = collection["DateToRepair"].ToString();
            if (collection["NewClient"].ToString() != "")
            {
                Clients client = new Clients
                {
                    Name = collection["NewClient"].ToString(),
                    ResponsiblePerson = collection["NewClient"].ToString()
                };
                dataBase.Clients.Add(client);
                dataBase.SaveChanges();
                clientid = client.Id;
            }

            Repairs repairs = new Repairs
            {
                ClientId = collection["NewClient"].ToString() == "" ? Convert.ToInt32(collection["Client"]) : clientid,
                ProductId = Convert.ToInt32(collection["Product"]),
                FactoryNumber = collection["FactoryNumber"].ToString(),
                RTUNumber = collection["RTUNumber"].ToString(),
                DateToRepair = Convert.ToDateTime(dateToRepair),
                Defect = collection["Defect"].ToString(),
                Set = collection["Set"].ToString(),
                Status = "accepted",
                Note = collection["Note"]
            };
            dataBase.Repairs.Add(repairs);
            dataBase.SaveChanges();
            Clients clients = dataBase.Clients.Find(Convert.ToInt32(collection["Client"]));
            var url = AppDomain.CurrentDomain.BaseDirectory + "App_Data/";
            var product = dataBase.Products.Where(p => p.Id == repairs.ProductId).Select(p => p.Name).FirstOrDefault();
            if (collection["NewClient"].ToString() == "")
            {

                fieldValues = new Dictionary<string, string> {
                {"Заказчик", clients.Name == null? " " :  clients.Name},
                {"Изделие", product == null? " " :  product.ToString()},
                {"Номер", repairs.FactoryNumber == null? " " :  repairs.FactoryNumber},
                {"RTU", repairs.RTUNumber == null? " " :  repairs.RTUNumber},
                {"Комплектность", repairs.Set == null? " " :  repairs.Set},
                {"Неисправность", repairs.Defect == null? " " :  repairs.Defect},
                {"ДАТА", repairs.DateToRepair.ToShortDateString()  == null? " " : repairs.DateToRepair.ToShortDateString() },
            };
                filename = clients.Name + repairs.DateToRepair.ToShortDateString() + ".docx";
                engine.Merge(url + "repair.docx", fieldValues, url + filename);
                return filename;
            }
            else
            {
                var client = collection["NewClient"].ToString();
                fieldValues = new Dictionary<string, string> {
                {"Заказчик", client},
                {"Изделие", product == null? " " :  product.ToString()},
                {"Номер", repairs.FactoryNumber == null? " " :  repairs.FactoryNumber},
                {"RTU", repairs.RTUNumber == null? " " :  repairs.RTUNumber},
                {"Комплектность", repairs.Set == null? " " :  repairs.Set},
                {"Неисправность", repairs.Defect == null? " " :  repairs.Defect},
                {"ДАТА", repairs.DateToRepair.Day.ToString()  == null? " " : repairs.DateToRepair.Day.ToString() },
            };
                filename = client + repairs.DateToRepair.ToShortDateString() + ".docx";
                engine.Merge(url + "repair.docx", fieldValues, url + filename);
                return filename;
            }
        }
        public void Update(int id, Dictionary<string, string> collection)
        {
            Repairs repairs = dataBase.Repairs.Find(id);
            repairs.Jobs = collection.ContainsKey("Jobs") ? collection["Jobs"].ToString() : null;
            repairs.Materials = collection.ContainsKey("Materials") ? collection["Materials"].ToString() : null;
            repairs.Devices = collection["Devices"].ToString();
            repairs.Status = "ready";
            repairs.Deleted = false;
            repairs.ResponsiblePersonId = Convert.ToInt32(collection["Repairer"]);
            dataBase.Entry(repairs).State = EntityState.Modified;
            dataBase.SaveChanges();
        }

        public void Delete(int Id)
        {
            Repairs repairs = dataBase.Repairs.Find(Id);
            repairs.Deleted = true;
            dataBase.Entry(repairs).State = EntityState.Modified;
            dataBase.SaveChanges();
        }

        public void Return(int id, Dictionary<string, string> collection)
        {
            var dateToClient = collection["DateToClient"].ToString();
            Repairs repairs = dataBase.Repairs.Find(id);
            repairs.Note = collection["Repairs.Note"].ToString();
            if(dateToClient != "")
            {
                repairs.DateToClient = Convert.ToDateTime(dateToClient);
                repairs.Status = "returned";
            }
            else
            {
                repairs.Status = "ready";
                repairs.DateToClient = DateTime.MinValue;
            }
            Update(repairs);
        }

        public void Edit(int id, Dictionary<string, string> collection)
        {
            var cl = collection["Client"].ToString();
            var p = collection["Product"].ToString();
            var dateToRepair = collection["Repairs.DateToRepair"].ToString();
            Repairs repairs = dataBase.Repairs.Find(id);
            var client = dataBase.Clients.Where(c => c.Name == cl).Select(c => c.Id).FirstOrDefault();
            repairs.ClientId = Convert.ToInt32(client);
            var product = dataBase.Products.Where(c => c.Name == p).Select(c => c.Id).FirstOrDefault();
            repairs.ProductId = Convert.ToInt32(product);
            repairs.FactoryNumber = collection["Repairs.FactoryNumber"].ToString();
            repairs.RTUNumber = collection["Repairs.RTUNumber"].ToString();
            repairs.DateToRepair = Convert.ToDateTime(dateToRepair);
            repairs.Defect = collection["Repairs.Defect"].ToString();
            repairs.Set = collection["Set"].ToString();
            repairs.Note = collection["Repairs.Note"].ToString();
            repairs.ContactPerson = collection["Repairs.ContactPerson"].ToString();
            Update(repairs);
        }

        public void EditJobs(int id, Dictionary<string, string> collection)
        {
            Repairs repairs = dataBase.Repairs.Find(id);
            repairs.Jobs = collection.ContainsKey("Jobs") ? collection["Jobs"].ToString() : null;
            repairs.Materials = collection.ContainsKey("Materials") ? collection["Materials"].ToString() : null;
            repairs.Devices = collection["Repairs.Devices"].ToString();
            repairs.ResponsiblePersonId = Convert.ToInt32(collection["Repairer"]);
            if (repairs.Materials == null && repairs.Jobs == null) repairs.Status = "accepted";
            if (collection.ContainsKey("RepairIsReady"))
            {
                if (collection["RepairIsReady"].ToString() == "on")
                    repairs.Status = "ready";
                else repairs.Status = "inprogress";
            }
            else repairs.Status = "inprogress";
            Update(repairs);
        }

        public ClientRepair CreateAcct(string ids)
        {
            ClientRepair clientRepair = new ClientRepair();
            double value = 0;
            foreach (var id in ids.Split(','))
            {
                RepairJobsAndMaterials repairJob = new RepairJobsAndMaterials();
                Repairs repairs = dataBase.Repairs.Find(Convert.ToInt32(id));
                RepairClass repair = new RepairClass
                {
                    Repairs = repairs
                };
                repairJob.ControllerName = "Контроллер " + repair.Products.
                    Where(r => r.Id == repairs.ProductId).FirstOrDefault().Name + " серийный номер " + repair.Repairs.FactoryNumber;
                if (repair.Repairs.Materials != null)
                {
                    foreach (var item in repairs.Materials.Split(','))
                    {
                        try
                        {
                            var materialID = Convert.ToInt32(item.ToString());
                            var accessoryID = dataBase.Accessories.FirstOrDefault(m => m.Id == materialID);
                            if (accessoryID != null)
                            {
                                var valueMat = dataBase.Accessories.Where(c => c.Id == accessoryID.Id).FirstOrDefault();
                                if (valueMat != null)
                                {
                                    repairJob.Accessories.Add(valueMat.Name);
                                    value += Convert.ToDouble(valueMat.Value.ToString().Replace('.', ','));
                                }
                            }
                        }
                        catch(Exception) { }
                    }
                }
                if (repair.Repairs.Jobs != null)
                {
                    foreach (var item in repairs.Jobs.Split(','))
                    {
                        var jobId = Convert.ToInt32(item.ToString());
                        var valueJob = dataBase.Jobs.Where(c => c.Id == jobId).FirstOrDefault();
                        if (valueJob != null)
                        {
                            repairJob.Jobs.Add(valueJob.Name);
                            value += Convert.ToDouble(valueJob.NormsOfTime.ToString().Replace('.', ','));
                        }
                    }
                }
                IEnumerable<Clients> clients = dataBase.Clients;
                IEnumerable<Repairs> rep = dataBase.Repairs;
                var client = from rp in rep
                             where rp.Id == Convert.ToInt32(id)
                             join prod in clients on rp.ClientId equals prod.Id
                             select prod;
                clientRepair.Client = client.First().Name;
                clientRepair.ClientContractNumber = client.First().ContractNumber;
                clientRepair.ClientLastContractDate = client.First().ContractDate;
                if ((client.First().ContractDate != null && client.First().ContractDate != ""))
                    clientRepair.ClientLastContractDate = String.Join("-", clientRepair.ClientLastContractDate.Split('.')).ToString();
                else
                    clientRepair.ClientLastContractDate = "";
                if ((client.First().ContractNumber != null && client.First().ContractNumber != ""))
                    clientRepair.ClientContractNumber = client.First().ContractNumber;
                else
                    clientRepair.ClientContractNumber = "";
                
                clientRepair.TResponsible.name = client.FirstOrDefault().LongResponsiblePerson;
                clientRepair.TResponsible.shortName = client.FirstOrDefault().ResponsiblePerson;
                clientRepair.TResponsible.JobTitle = client.FirstOrDefault().RPJobTitle;
                clientRepair.RepairAndMaterials.Add(repairJob);
            }
            clientRepair.ValueForJob = value.ToString();
            clientRepair.Ids = ids;
            return clientRepair;
        }
      
        public String AcctToExcel(ResponsiblePerson OurRes, ResponsiblePerson RPersOther, List<int> ids,
            List<NormsOfTime> normsOfTime, string ActInDoc, string DateToAcct,  string LastContractNumber, string LastContractDate, string AcctNumber)
        {
            var ourId = dataBase.ResponsiblePersons.Where(rp => rp.name == OurRes.name).FirstOrDefault();
            ourId.name = OurRes.name;
            ourId.shortName = OurRes.shortName;
            ourId.Base = OurRes.Base;
            ourId.JobTitle = OurRes.JobTitle;
            Update(ourId);
            List<RepairViewModel> repairsForExport = new List<RepairViewModel>();
            Repairs repairs = dataBase.Repairs.Find(ids.FirstOrDefault());
            List<Repairs> SelectedRepairsForSimmilarClient = new List<Repairs>();
            List<ResponsiblePersonsProperties> personsProperties = new List<ResponsiblePersonsProperties>();
            Clients client = new Clients();
            Dictionary<string, int> DeviceTypeAndCount = new Dictionary<string, int>();
            //var dateToActFromat = Convert.ToDateTime(DateToAcct);
            //DateToAcct = dateToActFromat.ToString("MM-dd-yyyy");
            foreach (int id in ids)
            {
                SelectedRepairsForSimmilarClient.Add(dataBase.Repairs.
                    Where(r => r.ClientId == repairs.ClientId && r.Id == id).FirstOrDefault());
            }
            foreach(Repairs repair in SelectedRepairsForSimmilarClient)
            {
                if(repair != null)
                {
                    RepairViewModel repairForExport = new RepairViewModel();
                    client = repairForExport.Clients.Where(c => c.Id == repairs.ClientId).FirstOrDefault();
                    List<Products> products = new List<Products>();
                    products.Add(dataBase.Products.Where(p => p.Id == repair.ProductId).FirstOrDefault());
                    repairForExport.Products = products;
                    List<JobsViewModel> jobsView = new List<JobsViewModel>();
                    List<Jobs> jobs = new List<Jobs>();
                    if (!string.IsNullOrWhiteSpace(repair.Jobs))
                    {
                        foreach (var item in repair.Jobs.Split(','))
                        {
                            var jobID = Convert.ToInt32(item);
                            jobs.Add(dataBase.Jobs.Where(j => j.Id == jobID).FirstOrDefault());
                        }
                        var groupedJobs = jobs.GroupBy(j => j.Id).Select(g => new
                        {
                            Id = g.Key,
                            Count = g.Count(),
                            Name = g.Select(j => j.Name).FirstOrDefault(),
                            NormsOfTime = g.Select(j => j.NormsOfTime).FirstOrDefault()
                        });
                        foreach (var j in groupedJobs)
                        {
                            jobsView.Add(new JobsViewModel()
                            {
                                Count = j.Count,
                                Id = j.Id,
                                Name = j.Name,
                                NormsOfTime = j.NormsOfTime
                            });
                        }
                    }
                    List<AccessoryViewModel> accessoriesView = new List<AccessoryViewModel>();
                    List<Accessories> accessories = new List<Accessories>();
                    if (!string.IsNullOrWhiteSpace(repair.Materials))
                    {
                        foreach (var item in repair.Materials.Split(','))
                        {
                            var materialID = Convert.ToInt32(item);
                            var accessoryID = dataBase.Accessories.FirstOrDefault(m => m.Id == materialID);
                            if (accessoryID != null)
                            {
                                if (accessoryID.Id != 0)
                                {
                                    var accessory = dataBase.Accessories.FirstOrDefault(a => a.Id == accessoryID.Id);
                                    if (accessory != null)
                                        if (accessory.Value != "0")
                                            accessories.Add(accessory);
                                }
                            }
                        }
                        if (accessories.Count != 0)
                        {
                            var groupedAccessories = accessories.GroupBy(a => a.Id).Select(g => new
                            {
                                Id = g.Key,
                                Count = g.Count(),
                                Name = g.Select(a => a.Name).FirstOrDefault(),
                                Value = g.Select(a => a.Value).FirstOrDefault(),
                                MeasureUnit = g.Select(a => a.MeasureUnit).FirstOrDefault()
                            });
                            foreach (var a in groupedAccessories)
                            {
                                accessoriesView.Add(new AccessoryViewModel()
                                {
                                    Count = a.Count,
                                    Id = a.Id,
                                    Name = a.Name,
                                    Value = a.Value,
                                    MeasureUnit = Convert.ToInt32(Regex.Match(a.MeasureUnit, @"\d+").Value),
                                });
                            }
                        }
                    }
                    var responsiblePerson = dataBase.ResponsiblePersonsProperties.Where(rp => rp.id == repair.ResponsiblePersonId).FirstOrDefault();
                    repairForExport.Jobs = jobsView;
                    repairForExport.Accessories = accessoriesView;
                    repairForExport.Repairs = repair;
                    repairForExport.ClientsToExport = client;
                    repairForExport.personProperties = responsiblePerson;
                    var deviceTypeName = dataBase.Products.Where(p => p.Id == repair.ProductId).FirstOrDefault().Name;
                    if (DeviceTypeAndCount.ContainsKey(deviceTypeName))
                    {
                        var deviceCount = DeviceTypeAndCount.Where(dt => dt.Key == deviceTypeName).FirstOrDefault().Value;
                        DeviceTypeAndCount.Remove(deviceTypeName);
                        DeviceTypeAndCount.Add(deviceTypeName, deviceCount + 1);
                    }
                    else
                    {
                        DeviceTypeAndCount.Add(deviceTypeName, 1);
                    }
                    repairsForExport.Add(repairForExport);
                }
            }
            return Export("Calculated", OurRes, RPersOther, repairsForExport, normsOfTime, AcctNumber, DateToAcct, ActInDoc, LastContractNumber, LastContractDate, DeviceTypeAndCount);
        }

        public String Export(String Template, ResponsiblePerson OurRes, ResponsiblePerson RPersOther, 
            List<RepairViewModel> repairs, List<NormsOfTime> normsOfTime,string Acct, string DateToAcct, string ActInDoc, string LastContractNumber, string LastContractDate, Dictionary<string, int> DeviceTypeAndCount)
        {
            string indicatorsJson = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "App_Data\\IndicatorConstants.json");
            var indicators = JsonConvert.DeserializeObject<IEnumerable<BaseIndicatorModel>>(indicatorsJson) as List<BaseIndicatorModel>;
            string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\";
            string exportFileName = Template + Guid.NewGuid().ToString() + ".xls"; // имя для файла экспорта
            string exportFilePath = path + exportFileName;

            System.IO.File.Copy(path + Template + ".xlt", exportFilePath); // копируем в файл шаблон

            string connectionString = ConfigurationManager.ConnectionStrings["reportconnectionstring"].ToString();

            connectionString = connectionString.Replace("{0}", "'" + exportFilePath + "'");
            //"provider=Microsoft.Jet.OLEDB.4.0;Data Source='" +
            //exportFilePath + "';Extended Properties=Excel 8.0;"; // строка подключения
            //
            using (OleDbConnection ole = new OleDbConnection(connectionString)) // используем OleDb
            {
                try
                {
                    ole.Open();

                    //вставка заголовков колонок
                    String stringCmd = "INSERT INTO [Данные$] ";
                    //строка с номерами колонок для оператора INSERT

                    stringCmd += "([repairID]," +
                        " [productName]," +
                        " [serialNumber]," +
                        " [RTUNumber]," +
                        " [jobID]," +
                        " [jobs]," +
                        " [jobsTime], " +
                        " [accessoryID]," +
                        " [accessories]," +
                        " [accValues]," +
                        " [accessoriesCount]," +
                        " [jobsCount], " +
                        " [dateToRepair]," +
                        " [acctNumber]," +
                        " [inputControl]," +
                        " [searchDefects]," +
                        " [restoreDefects], " +
                        " [updateOS]," +
                        " [generalCheck]," +
                        " [sellaryPerMonth]," +
                        " [avgWorkTime]," +
                        " [hourPrice]," +
                        " [FSZNdeductions]," +
                        " [compulsoryInsurance]," +
                        " [generalRunningCosts]," +
                        " [plannedSavings]," +
                        " [VAT]," +
                        " [measureUnit]," +
                        " [NameOfClient]," +
                        " [AddressOfClient], " +
                        " [CheckingAccount], " +
                        " [Bank], " +
                        " [BankAddress], " +
                        " [BIC], " +
                        " [UNP]," +
                        " [OKPO]," +
                        " [Phone]," +
                        " [Email], " +
                        " [CRPShort], " +
                        " [CRPLong], " +
                        " [CRPTitle], " +
                        " [ORPShort], " +
                        " [ORPLong], " +
                        " [ORPTitle], " +
                        " [ActNumberInDoc], " +
                        " [LastContractDate], " +
                        " [LastContractNumber], " +
                        " [RepairDeviceType], " +
                        " [RepairDeviceCount] " +
                        ") " +
                        "VALUES(" +
                        "'Идентификатор'," +
                        "'Изделие'," +
                        "'Серийный номер'," +
                        "'Номер RTU'," +
                        "'Идентификатор выполненной работы'" +
                        ",'Выполненные работы'," +
                        "'Время выполненной работы'," +
                        "'Идентификатор установленного комплектующего'," +
                        "'Использованные материалы'," +
                        "'Цена за комплектующие'," +
                        "'Количество установленных комплектующих'," +
                        "'Количество выполненной работы'," +
                        "'Дата выдачи', " +
                        "'Номер акта', " +
                        "'Входной контроль', " +
                        "'Поиск неисправностей'," +
                        "'Устранение неисправностей', " +
                        "'Обновление программного обеспечения', " +
                        "'Регулирование, проверка'," +
                        "'Должностной оклад'," +
                        "'Средняя норма рабочего времени'," +
                        "'Стоимость 1 часа'," +
                        "'Отчисления ФСЗН'," +
                        "'Обязательное страхование'," +
                        "'Общехозяйственные расходы'," +
                        "'Плановые накопления'," +
                        "'НДС'," +
                        "'Единица измерения комплектующего'," +
                        "'Наименование получателя'," +
                        "'Адрес'," +
                        "'Расчетный счет'," +
                        "'Банк'," +
                        "'Адрес банка'," +
                        "'BIC'," +
                        "'UNP'," +
                        "'ОКПО'," +
                        "'Телефоны'," +
                        "'Email'," +
                        "'Ответственный с нашей стороны ИП'," +
                        "'Ответственный с нашей стороны РП'," +
                        "'Должность'," +
                        "'Ответственный с другой стороны ИП'," +
                        "'Ответственный с другой стороны РП'," +
                        "'Должность с другой стороны'," +
                        "'Номер акта договора'," +
                        "'Последняя дата договора'," +
                        "'Последний номер договора'," +
                        "'Тип отремонтированного прибора'," +
                        "'Количество приборов по типу'" +
                        ")";
                    using (OleDbCommand cmd = new OleDbCommand(stringCmd, ole))
                    {
                        try
                        {
                            cmd.ExecuteNonQuery();//вставка данных 
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    int idx = 0;
                    foreach (RepairViewModel repair in repairs)
                    {
                        var productName = repair.Products.FirstOrDefault();
                        foreach (var j in repair.Jobs)
                        {
                            stringCmd = "INSERT INTO [Данные$] " +
                            "(" +
                            " [repairID]," +
                            " [productName]," +
                            " [serialNumber], " +
                            " [RTUNumber], " +
                            " [jobID]," +
                            " [jobs]," +
                            " [jobsTime]," +
                            " [accessoryID]," +
                            " [accessories], " +
                            " [accValues]," +
                            " [accessoriesCount]," +
                            " [jobsCount]," +
                            " [dateToRepair]," +
                            " [acctNumber]," +
                            " [inputControl]," +
                            " [searchDefects]," +
                            " [restoreDefects], " +
                            " [updateOS]," +
                            " [generalCheck]," +
                            " [sellaryPerMonth]," +
                            " [avgWorkTime]," +
                            " [hourPrice]," +
                            " [FSZNdeductions]," +
                            " [compulsoryInsurance]," +
                            " [generalRunningCosts], " +
                            " [plannedSavings]," +
                            " [VAT]," +
                            " [measureUnit], " +
                            " [NameOfClient], " +
                            " [AddressOfClient], " +
                            " [CheckingAccount], " +
                            " [Bank], " +
                            " [BankAddress], " +
                            " [BIC], " +
                            " [UNP], " +
                            " [OKPO], " +
                            " [Phone], " +
                            " [Email], " +
                            " [CRPShort], " +
                            " [CRPLong], " +
                            " [CRPTitle], " +
                            " [ORPShort], " +
                            " [ORPLong], " +
                            " [ORPTitle], " +
                            " [ActNumberInDoc], " +
                            " [LastContractDate], " +
                            " [LastContractNumber], " +
                            " [RepairDeviceType], " +
                            " [RepairDeviceCount] " +
                            ")" +
                              "VALUES("
                                + repair.Repairs.Id + ",'"
                                + productName.Name + "','"
                                + repair.Repairs.FactoryNumber + "','"
                                + repair.Repairs.RTUNumber + "',"
                                + j.Id + ",'"
                                + j.Name + "','"
                                + Convert.ToDouble(j.NormsOfTime.Replace('.', ',')).ToString() + "',"
                                + "0, " +
                                "''," +
                                " 0,"
                                + "0,"
                                + j.Count + ", '"
                                + DateToAcct + "','"
                                + Acct + "','"
                                + normsOfTime[idx].InputControl.ToString() + "','"
                                + normsOfTime[idx].SearchDefects.ToString() + "','"
                                + normsOfTime[idx].RestoreDefects.ToString() + "','"
                                + normsOfTime[idx].UpdateOS.ToString() + "','"
                                + normsOfTime[idx].Check.ToString() + "','"
                                + repair.personProperties.sellaryPerMonth + "','"
                                + repair.personProperties.avgWorkTime + "','"
                                + repair.personProperties.hourPrice + "','"
                                + indicators[0].Value + "','"
                                + indicators[1].Value + "','"
                                + indicators[2].Value + "','"
                                + indicators[3].Value + "','"
                                + indicators[4].Value + "','"
                                + "0', '" 
                                + repair.ClientsToExport.Name + "','"
                                + repair.ClientsToExport.Address + "','"
                                + repair.ClientsToExport.CheckingAccount + "','"
                                + repair.ClientsToExport.Bank + "','"
                                + repair.ClientsToExport.BankAddress + "','"
                                + repair.ClientsToExport.BIC + "','"
                                + repair.ClientsToExport.UNP + "','"
                                + repair.ClientsToExport.OKPO + "','"
                                + repair.ClientsToExport.ContactNumber + "','"
                                + repair.ClientsToExport.Email + "','" 
                                + OurRes.shortName + "','" 
                                + OurRes.name + "','" 
                                + OurRes.JobTitle + "','" 
                                + RPersOther.shortName + "','" 
                                + RPersOther.name + "','" 
                                + RPersOther.JobTitle + "','" 
                                + ActInDoc + "','"
                                + LastContractDate + "','"
                                + LastContractNumber + "','" 
                                + productName.Name + "','" 
                                + DeviceTypeAndCount.Where(d => d.Key == productName.Name).FirstOrDefault().Value + "'" +
                                ")";
                            using (OleDbCommand cmd = new OleDbCommand(stringCmd, ole))
                            {
                                try
                                {
                                    cmd.ExecuteNonQuery();//вставка данных 
                                }
                                catch (Exception e)
                                {
                                }
                            }
                        };
                        foreach (var a in repair.Accessories)
                        {
                            double accessoryCount = (double)a.Count / (double)a.MeasureUnit;
                            stringCmd = "INSERT INTO [Данные$] " +
                            "(" +
                            " [repairID]," +
                            " [productName]," +
                            " [serialNumber], " +
                            " [RTUNumber], " +
                            " [jobID]," +
                            " [jobs]," +
                            " [jobsTime]," +
                            " [accessoryID]," +
                            " [accessories], " +
                            " [accValues]," +
                            " [accessoriesCount]," +
                            " [jobsCount]," +
                            " [dateToRepair]," +
                            " [acctNumber]," +
                            " [inputControl]," +
                            " [searchDefects]," +
                            " [restoreDefects], " +
                            " [updateOS]," +
                            " [generalCheck]," +
                            " [sellaryPerMonth]," +
                            " [avgWorkTime]," +
                            " [hourPrice]," +
                            " [FSZNdeductions]," +
                            " [compulsoryInsurance]," +
                            " [generalRunningCosts], " +
                            " [plannedSavings]," +
                            " [VAT]," +
                            " [measureUnit], " +
                            " [NameOfClient], " +
                            " [AddressOfClient], " +
                            " [CheckingAccount], " +
                            " [Bank], " +
                            " [BankAddress], " +
                            " [BIC], " +
                            " [UNP], " +
                            " [OKPO], " +
                            " [Phone], " +
                            " [Email], " +
                            " [CRPShort], " +
                            " [CRPLong], " +
                            " [CRPTitle], " +
                            " [ORPShort], " +
                            " [ORPLong], " +
                            " [ORPTitle], " +
                            " [ActNumberInDoc], " +
                            " [LastContractDate], " +
                            " [LastContractNumber], " +
                            " [RepairDeviceType], " +
                            " [RepairDeviceCount] " +
                            ")" +
                                "VALUES("
                                + repair.Repairs.Id + ",'"
                                + productName.Name + "','"
                                + repair.Repairs.FactoryNumber + "','"
                                + repair.Repairs.RTUNumber
                                + "', 0," +
                                  "''," +
                                  " 0," +
                                +a.Id + ",'"
                                + a.Name + "','"
                                + Convert.ToDouble(a.Value.Replace('.', ',')).ToString() + "','" +
                                  accessoryCount.ToString().Replace('.', ',') +
                                  "'," +
                                  " 0, '"
                                + DateToAcct + "','"
                                + Acct + "','"
                                + normsOfTime[idx].InputControl.ToString() + "','"
                                + normsOfTime[idx].SearchDefects.ToString() + "','"
                                + normsOfTime[idx].RestoreDefects.ToString() + "','"
                                + normsOfTime[idx].UpdateOS.ToString() + "','"
                                + normsOfTime[idx].Check.ToString() + "','" +
                                + repair.personProperties.sellaryPerMonth + "','"
                                + repair.personProperties.avgWorkTime + "','"
                                + repair.personProperties.hourPrice + "','"
                                + indicators[0].Value + "','"
                                + indicators[1].Value + "','"
                                + indicators[2].Value + "','"
                                + indicators[3].Value + "','"
                                + indicators[4].Value + "','"
                                + a.MeasureUnit + "','"
                                + repair.ClientsToExport.Name + "','"
                                + repair.ClientsToExport.Address + "','"
                                + repair.ClientsToExport.CheckingAccount + "','"
                                + repair.ClientsToExport.Bank + "','"
                                + repair.ClientsToExport.BankAddress + "','"
                                + repair.ClientsToExport.BIC + "','"
                                + repair.ClientsToExport.UNP + "','"
                                + repair.ClientsToExport.OKPO + "','"
                                + repair.ClientsToExport.ContactNumber + "','"
                                + repair.ClientsToExport.Email + "','"
                                + OurRes.shortName + "','"
                                + OurRes.name + "','"
                                + OurRes.JobTitle + "','"
                                + RPersOther.shortName + "','"
                                + RPersOther.name + "','"
                                + RPersOther.JobTitle + "','"
                                + ActInDoc + "','"
                                + LastContractDate + "','"
                                + LastContractNumber + "','"
                                + productName.Name + "','"
                                + DeviceTypeAndCount.Where(d => d.Key == productName.Name).FirstOrDefault().Value + "'" +
                                ")";
                            using (OleDbCommand cmd = new OleDbCommand(stringCmd, ole))
                            {
                                try
                                {
                                    cmd.ExecuteNonQuery();//вставка данных 
                                }
                                catch (Exception e)
                                {
                                }
                            }
                        };
                        idx++;
                    }
                }
                catch (Exception e)
                {
                }
                finally
                {
                    ole.Close();
                }
            }
            return exportFileName;
        }
    }
}