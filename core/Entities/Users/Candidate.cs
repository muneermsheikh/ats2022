using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace core.Entities.Users
{
    public class Candidate: BaseEntity
    {
          public Candidate()
          {
          }

          public Candidate(string gender, int appUserId,  bool appUserIdNotEnforced, int applicationNo, 
                string firstName, string secondName, string familyName, string knownAs, DateTime dOB, 
                string placeOfBirth, string aadharNo, string email, string introduction, 
                string interests, bool notificationDesired, string nationality, 
                int companyId, string ppno, string city, string pin, int referredby,
                
                ICollection<UserQualification> userQualifications, 
                ICollection<UserProfession> userProfessions, ICollection<UserPassport> userPassports, 
                ICollection<UserAttachment> userAttachments)
            {
                Gender = gender;
                AppUserId = appUserId;
                AppUserIdNotEnforced = appUserIdNotEnforced;
                ApplicationNo = applicationNo;
                FirstName = firstName;  SecondName = secondName; FamilyName=familyName;
                KnownAs= knownAs; DOB = dOB; AadharNo=aadharNo; PpNo = ppno; Email = email;
                PlaceOfBirth = placeOfBirth; 
                Introduction = introduction;
                Interests = interests;
                City = city; Pin=pin; ReferredBy = referredby;
                Nationality = nationality; NotificationDesired = notificationDesired;
                UserQualifications = userQualifications;
                UserProfessions = userProfessions;
                UserPassports = userPassports;
                UserAttachments = userAttachments;
                
            }

        public int ApplicationNo { get; set; }
//names                
        [Required, MaxLength(1)]
        public string Gender {get; set;}="M";
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        [Required]         
        public string KnownAs { get; set; }
        public int ReferredBy { get; set; }
        public string Source {get; set;}
        [Required]
        public DateTime DOB { get; set; }
        public string PlaceOfBirth { get; set; }
        public string AadharNo { get; set; }
        public string PpNo { get; set; } 
        public bool Ecnr { get; set; }
        public string City {get; set;}
        public string Pin {get; set;}
        public string Nationality {get; set;}
        [EmailAddress]
        public string Email { get; set; }
    //general
        public int? CompanyId {get; set;}       //associate id
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string Interests { get; set; }
    //navigations
        public bool AppUserIdNotEnforced { get; set; }
        public int AppUserId {get; set;}
        public bool NotificationDesired {get; set;}

        public string CandidateStatus {get; set;}="Not Referrred";
        public string PhotoUrl { get; set; }
        public ICollection<UserPhone> UserPhones {get; set;}
        public ICollection<UserQualification> UserQualifications { get; set; }
        public ICollection<UserProfession> UserProfessions {get; set;}
        public ICollection<UserPassport> UserPassports {get; set;}
        public ICollection<UserAttachment> UserAttachments { get; set; }
        //public ICollection<IFormFile> UserFormFiles {get; set;}
        public ICollection<EntityAddress> EntityAddresses {get; set;}
        public ICollection<UserExp> UserExperiences {get; set;}
        public ICollection<Photo> UserPhotos {get; set;}
        public string FullName {get => FirstName + " " + SecondName + " " + FamilyName;}
    }
}