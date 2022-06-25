using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.Admin;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Entities.Process;
using core.Entities.Tasks;
using core.Entities.Users;
using Microsoft.Extensions.Logging;

namespace infra.Data
{
    public class ATSContextSeed
    {
        public static async Task SeedAsync(ATSContext context, ILoggerFactory loggerFactory)
        {
            try {
                // all dependent, i.e. master, table seeding should be done first
                if (!context.ContactResults.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ContactResultSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ContactResult>>(jsonData);
                    foreach(var item in fileData) {
                        context.ContactResults.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                
                if(!context.Categories.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/CategorySeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Category>>(jsonData);
                    foreach(var item in fileData) {
                        context.Categories.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.Industries.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/IndustrySeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Industry>>(jsonData);
                    foreach(var item in fileData) {
                        context.Industries.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.Qualifications.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/QualificationSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Qualification>>(jsonData);
                    foreach(var item in fileData) {
                        context.Qualifications.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                
                if(!context.ReviewItemStatuses.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ReviewItemStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ReviewItemStatus>>(jsonData);
                    foreach(var item in fileData) {
                        context.ReviewItemStatuses.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.ReviewStatuses.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ReviewStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ReviewStatus>>(jsonData);
                    foreach(var item in fileData) {
                        context.ReviewStatuses.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.SelectionStatuses.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/SelectionStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<SelectionStatus>>(jsonData);
                    foreach(var item in fileData) {
                        context.SelectionStatuses.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.DeployStatus.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/DeployStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<DeployStatus>>(jsonData);
                    foreach(var item in fileData) {
                        context.DeployStatus.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.MessageComposeSources.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/MessageComposeSourceSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<MessageComposeSource>>(jsonData);
                    foreach(var item in fileData) {
                        context.MessageComposeSources.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.TaskTypes.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/TaskTypeSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<TaskType>>(jsonData);
                    foreach(var item in fileData) {
                        context.TaskTypes.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.MessageTypes.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/MessageTypeSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<MessageType>>(jsonData);
                    foreach(var item in fileData) {
                        context.MessageTypes.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.ReviewItemDatas.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ReviewItemDataSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ReviewItemData>>(jsonData);
                    foreach(var item in fileData) {
                        context.ReviewItemDatas.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.ChecklistHRDatas.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ChecklistHRDataSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ChecklistHRData>>(jsonData);
                    foreach(var item in fileData) {
                        context.ChecklistHRDatas.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
            

                if(!context.AssessmentStandardQs.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/AssessmentStddQsSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<AssessmentStandardQ>>(jsonData);
                    foreach(var item in fileData) {
                        context.AssessmentStandardQs.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
            
                if(!context.SkillDatas.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/SkillSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<SkillData>>(jsonData);
                    foreach(var item in fileData) {
                        context.SkillDatas.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.InterviewAttendancesStatus.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/InterviewAttendanceStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<InterviewAttendanceStatus>>(jsonData);
                    foreach(var item in fileData) {
                        context.InterviewAttendancesStatus.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.Employees.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/EmployeeSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Employee>>(jsonData);
                    foreach(var item in fileData) {
                        context.Employees.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
        
                if(!context.Customers.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/CustomerSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Customer>>(jsonData);
                    foreach(var item in fileData) {
                        context.Customers.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.CustomerOfficials.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/CustomerOfficialSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<CustomerOfficial>>(jsonData);
                    foreach(var item in fileData) {
                        context.CustomerOfficials.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.Candidates.Any()) {
                    var jsonData1 = File.ReadAllText("../infra/data/SeedData/UserSeedData.json");
                    var fileData1 = JsonSerializer.Deserialize<List<Candidate>>(jsonData1);
                    foreach(var item in fileData1) {
                        context.Candidates.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                

                if(!context.Orders.Any()) 
                {
                    var jsnData = File.ReadAllText("../infra/data/SeedData/OrderSeedData.json");
                    var filData = JsonSerializer.Deserialize<List<Order>>(jsnData);
                    foreach(var item in filData) {
                        context.Orders.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.CVRefs.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/CVRefSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<CVRef>>(jsonData);
                    foreach(var item in fileData) {
                        context.CVRefs.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                
                /*
                if (context.CVRefs.Any()) {
                    if(!context.SelectionDecisions.Any()) {
                        var jsonData = File.ReadAllText("../infra/data/SeedData/SelectionDecisionSeedData.json");
                        var fileData = JsonSerializer.Deserialize<List<SelectionDecision>>(jsonData);
                        foreach(var item in fileData) {
                            context.SelectionDecisions.Add(item);
                        }

                        if(!context.Employments.Any()) 
                        {
                            var jsonD = File.ReadAllText("../infra/data/SeedData/EmploymentSeedData.json");
                            var fileD = JsonSerializer.Deserialize<List<Employment>>(jsonData);
                            foreach(var em in fileD) {
                                context.Employments.Add(em);
                            }
                        }
                        await context.SaveChangesAsync();
                    }
                }
                */

            } catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<ATSContextSeed>();
                logger.LogError("reached in catch exception loop");
                logger.LogError(ex.Message);
            } 
        }
    }
}
