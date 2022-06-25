using System.Linq;
using api.Errors;
using core.Interfaces;
using infra.Data;
using infra.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace api.Extensions
{
    public static class ApplicationServiceExtensions
    {
            public static IServiceCollection AddApplicationServices(this IServiceCollection services)
            {
                //services.AddSingleton<IResponseCacheService, ResponseCacheService>();
                
                services.AddScoped<ITokenService, TokenService>();
                services.AddScoped<ICategoryRepository, CategoryRepository>();
                services.AddScoped<ICustomerReviewService, CustomerReviewService>();
                
                //services.AddScoped<IMessageRepository, MessageRepository>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddScoped<IOrderService, OrderService>();
                services.AddScoped<IOrderItemService, OrderItemService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<ICustomerService, CustomerService>();
                services.AddScoped<IMastersService, MastersService>();
                services.AddScoped<IEmployeeService, EmployeeService>();
                services.AddScoped<IOrderAssessmentService, OrderAssessmentService>();
                services.AddScoped<ICandidateAssessmentService, CandidateAssessmentService>();
                services.AddScoped<ICVRefService, CVRefService>();
                services.AddScoped<ISelectionDecisionService, SelectionDecisionService>();
                services.AddScoped<IEmploymentService, EmploymentService>();
                services.AddScoped<IStatsService, StatsService>();
                services.AddScoped<IContractReviewService, ContractReviewService>();
                services.AddScoped<IDeployService, DeployService>();
                services.AddScoped<ISMSService, SMSService>();
                services.AddScoped<IEmailService, EmailService>();
                services.AddScoped<ICommonServices, CommonServices>();
                services.AddScoped<IComposeMessages, ComposeMessages>();
                services.AddScoped<IComposeMessagesForAdmin, ComposeMessagesForAdmin>();
                services.AddScoped<IComposeMessagesForHR, ComposeMessagesForHR>();
                services.AddScoped<IComposeMessagesForProcessing, ComposeMessagesForProcessing>();
                services.AddScoped<ITaskService, TaskServices>();
                services.AddScoped<ICVReviewService, CVRvwService>();
                services.AddScoped<IVerifyService, VerifyService>();
                services.AddScoped<IOrderAssignmentService, OrderAssignmentService>();
                services.AddScoped<IChecklistService, ChecklistService>();
                services.AddScoped<IInterviewService, InterviewService>();
                services.AddScoped<IInterviewFollowupService, InterviewFollowupService>();
                services.AddScoped<IUserHistoryService, UserHistoryService>();
                services.AddScoped<IDLForwardService, DLForwardService>();
                services.AddScoped<IAssessmentQBankService, AssessmentQBankService>();
                services.AddScoped<IAssessmentStandardQService, AssessmentStandardQService>();
                services.AddScoped<IProspectiveCandidateService, ProspectiveCandidateService>();
                services.AddScoped<IValidateTaskService, ValidateTaskService>();
                services.AddScoped<IComposeMessageForCandidates, ComposeMessageForProspectiveCandidateService>();
                services.AddScoped<IComposeMessagesForInternalReviewHR, ComposeMessagesForInternalReviewHR>();
                services.AddScoped<ITaskControlledService, TaskControlledServices>();
                services.AddScoped<IComposeOrderAssessment, ComposeOrderAssessment>();
                services.AddScoped<ICategoryService, CategoryService>();

                services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
            
                services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        var errors = actionContext.ModelState
                            .Where(e => e.Value.Errors.Count > 0)
                            .SelectMany(x => x.Value.Errors)
                            .Select(x => x.ErrorMessage).ToArray();

                        var errorResponse = new ApiValidationErrorResponse
                        {
                            Errors = errors
                        };

                        return new BadRequestObjectResult(errorResponse);
                    };
                });

                services.AddAuthorization(opt => {
                    opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                    opt.AddPolicy("RequireHRSupervisorRole", policy => policy.RequireRole("HRSupervisor"));
                    opt.AddPolicy("RequirePPReleaseRole", policy => policy.RequireRole("PPRelease"));
                    opt.AddPolicy("RequireProcessingManagerRole", policy => policy.RequireRole("ProcessingManager"));
                    opt.AddPolicy("RequireRegisterSelectionRole", policy => policy.RequireRole("AddEditSelection"));
                });

                
                return services;
            }
    }
}