using Project.Models;

namespace Project.Domain
{
    public class JobEntityClass : BaseEntityClass
    {
        public Jobs GetJobs(int id)
        {
            var jobs = dataBase.Jobs.Find(id);
            return jobs;
        }
        public void Create(string JobName, string JobTime)
        {
            Jobs jobs = new Jobs
            {
                Name = JobName,
                NormsOfTime = JobTime,
               
            };
            dataBase.Jobs.Add(jobs);
            dataBase.SaveChanges();
        }
       
        public void Update(Jobs jobs)
        {
            Update(jobs as object);
        }
        public void Delete(Jobs jobs)
        {
            Delete(jobs as object);
        }
       
    }
}