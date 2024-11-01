using EasyDox;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Domain
{

    public class ClientEntityClass : BaseEntityClass
    {
        public Clients GetClient(int id)
        {
            Clients clients = dataBase.Clients.Find(id);
            return clients;
        }
        public ContractClass GetDataForContracts(int id)
        {
            ContractClass contractData = new ContractClass();
            contractData.Clients = GetClient(id);
            contractData.Person = dataBase.ResponsiblePersons;
            return contractData;
        }
        public void Create(Dictionary<string, string> collection)
        {
            Clients clients = new Clients
            {
                Name = collection["ClientName"],
                Address = collection["ClientAddress"].ToString() == "" ? "" : collection["ClientAddress"].ToString(),
                CheckingAccount = collection["ClientCheckingAccount"].ToString() == "" ? "" : collection["ClientCheckingAccount"].ToString(),
                Bank = collection["ClientBank"].ToString() == "" ? "" : collection["ClientBank"].ToString(),
                BankAddress = collection["ClientBankAddress"].ToString() == "" ? "" : collection["ClientBankAddress"].ToString(),
                BIC = collection["ClientBIC"].ToString() == "" ? "" : collection["ClientBIC"].ToString(),
                UNP = collection["ClientUNP"].ToString() == "" ? "" : collection["ClientUNP"].ToString(),
                OKPO = collection["ClientOKPO"].ToString() == "" ? "" : collection["ClientOKPO"].ToString(),
                ResponsiblePerson = collection["ClientResponsiblePerson"].ToString() == "" ? "" : collection["ClientResponsiblePerson"].ToString(),
                ContactNumber = collection["ClientContactNumber"].ToString() == "" ? "" : collection["ClientContactNumber"].ToString(),
                ContractDate = collection["ContractDate"].ToString() == "" ? "" : collection["ContractDate"].ToString(),
                ContractNumber = collection["ContractNumber"].ToString() == "" ? "" : collection["ContractNumber"].ToString(),
                LongResponsiblePerson = collection["LongResponsiblePerson"].ToString() == "" ? "" : collection["LongResponsiblePerson"].ToString(),
                RPJobTitle = collection["RPJobTitle"].ToString() == "" ? "" : collection["RPJobTitle"].ToString(),
                BasedOnDescription = collection["BasedOnDescription"].ToString() == "" ? "" : collection["BasedOnDescription"].ToString(),
                Email = collection["ClientEmail"].ToString() == "" ? "" : collection["ClientEmail"].ToString()
            };
            dataBase.Clients.Add(clients);
            dataBase.SaveChanges();
        }
        public String Contract(FormCollection client)
        {
            try
            {
                var engine = new Engine();
                var fieldValues = new Dictionary<string, string>();
                Clients clients = dataBase.Clients.Find(Convert.ToInt32(client["Clients.Id"]));
                ResponsiblePerson responsible = dataBase.ResponsiblePersons.Find(Convert.ToInt32(client["ResponsiblePersonId"]));
                if (clients != null && responsible != null)
                {
                    clients.ContractNumber = client["Clients.ContractNumber"];
                    clients.ContractDate = client["Clients.ContractDate"].ToString();
                    clients.RPJobTitle = client["Clients.RPJobTitle"].ToString();
                    clients.ResponsiblePerson = client["Clients.ResponsiblePerson"].ToString();
                    clients.LongResponsiblePerson = client["Clients.LongResponsiblePerson"].ToString();
                    DateTime endOfcontract = Convert.ToDateTime(client["ContractExpDate"].ToString());
                    fieldValues = new Dictionary<string, string>
                    {
                        {
                            //"Номер договора"
                            "ContractNumber",
                                client["Clients.ContractNumber"] != null &&
                                client["Clients.ContractNumber"] != "" ?
                                client["Clients.ContractNumber"] : ""
                        },
                        {
                            //"Дата договора"
                            "ContractDate",
                                client["Clients.ContractDate"] != null &&
                                client["Clients.ContractDate"] != "" ?
                                Convert.ToDateTime(client["Clients.ContractDate"]).ToString("dd MMMM yyyy") : ""
                        },
                        {
                            //"В лице наша организация"
                            "Our jobTitle",
                                responsible.JobTitle != null &&
                                responsible.JobTitle != "" ?
                                responsible.JobTitle : ""
                        },
                        {
                            //"Ответственное лицо нашей организации"
                            "Our responsible person",
                                responsible.name != null &&
                                responsible.name != "" ?
                                responsible.name : ""
                        },
                        {
                            //"Основание"
                            "Ground",
                                responsible.Base != null &&
                                responsible.Base != "" ?
                                responsible.Base : ""
                        },
                        {
                            //"Наименование организации"
                            "Client name",
                                clients.Name != null &&
                                clients.Name != "" ?
                                clients.Name : ""
                        },
                        {
                            //"В лице сторона заказчика"
                            "In person customer",
                                client["Clients.RPJobTitle"] != null &&
                                client["Clients.RPJobTitle"] != "" ?
                                client["Clients.RPJobTitle"].ToString() : ""
                        },
                        {
                            //"Ответственное лицо стороны заказчика РП"
                            "Responsible person RP",
                                client["Clients.LongResponsiblePerson"] != null &&
                                client["Clients.LongResponsiblePerson"] != "" ?
                                client["Clients.LongResponsiblePerson"].ToString() : ""
                        },
                        {
                            //"Конец договора"
                            "End date",
                                client["Clients.ContractDate"] != null &&
                                client["Clients.ContractDate"] != "" ?
                                endOfcontract.ToString("dd MMMM yyyy") : ""
                        },
                        //Заказчик
                        {
                            //"Наименование"
                            "Name",
                                clients.Name != null &&
                                clients.Name != "" ?
                                clients.Name : ""
                        },
                        {
                            //"Адрес"
                            "Address",
                                clients.Address != null &&
                                clients.Address != "" ?
                                clients.Address : ""
                        },
                        {
                            //"Расчетный счет"
                            "Checking account",
                                clients.CheckingAccount != null &&
                                clients.CheckingAccount != "" ?
                                clients.CheckingAccount : ""
                        },
                        {
                            //"Банк"
                            "Bank",
                                clients.Bank != null &&
                                clients.Bank != "" ?
                                clients.Bank : ""
                        },
                        {
                            //"Адрес банка"
                            "Bank address",
                                clients.BankAddress != null &&
                                clients.BankAddress != "" ?
                                clients.BankAddress : ""
                        },
                        {
                            //"BIC"
                            "BIC",
                                clients.BIC != null &&
                                clients.BIC != "" ?
                                clients.BIC : ""
                        },
                        {
                            //"УНП"
                            "UNP",
                                clients.UNP != null &&
                                clients.UNP != "" ?
                                clients.UNP : ""
                        },
                        {
                            //"ОКПО"
                            "OKPO",
                                clients.OKPO != null &&
                                clients.OKPO != "" ?
                                clients.OKPO : ""
                        },
                        {
                            //"Телефоны"
                            "Phones",
                                clients.ContactNumber != null &&
                                clients.ContactNumber != "" ?
                                clients.ContactNumber : ""
                        },
                        {
                            //"Email"
                            "Email",
                                clients.Email != null &&
                                clients.Email != "" ?
                                clients.Email : ""
                        },
                        {
                            //"Ответственное лицо стороны заказчика ИП"
                            "Responsible person customer IP",
                                client["Clients.ResponsiblePerson"] != null &&
                                client["Clients.ResponsiblePerson"] != "" ?
                                client["Clients.ResponsiblePerson"].ToString() : ""
                        },
                        {
                            //"Ответственное лицо с нашей стороны ИП"
                            "Our responsible person IP",
                                responsible.shortName != null &&
                                responsible.shortName != "" ?
                                responsible.shortName : ""
                        },
                        {
                            "BasedOnDescription",
                                clients.BasedOnDescription != null &&
                                clients.BasedOnDescription != "" ? 
                                clients.BasedOnDescription : "_______________" 
                        },
                    };
                    Update(clients);
                    var filename = "";
                    var url = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\";
                    filename = "Договор на ремонт " + clients.Name.Replace("\"", "") + " " + 
                        Convert.ToDateTime(clients.ContractDate).ToShortDateString() + ".docx";
                    engine.Merge(url + "repairContract.docx", fieldValues, url + filename);
                    return filename;
                }
            }
            catch(Exception e)
            {
                return "";
            }
            return "";
        }
        public void Update(Clients clients)
        {
            Update(clients as object);
        }
        public void Delete(Clients clients)
        {
            Delete(clients as object);
        }
    }
}