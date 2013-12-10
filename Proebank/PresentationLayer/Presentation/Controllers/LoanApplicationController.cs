﻿using System.Net;
using System.Web.Configuration;
using Domain.Enums;
using Domain.Models.Customers;
using Domain.Models.Loans;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Application;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class LoanApplicationController : BaseController
    {
        private readonly ProcessingService _service;

        public LoanApplicationController()
        {
            _service = new ProcessingService();
        }

        [Authorize(Roles = "Department head, Consultant, Security, Credit committee")]
        public ActionResult Index()
        {
            if (User.IsInRole("Consultant"))
            {
                return RedirectToAction("New");
            }
            if (User.IsInRole("Security"))
            {
                return RedirectToAction("OnSecurityReview");
            }
            if (User.IsInRole("Credit committee"))
            {
                return RedirectToAction("OnCommitteeReview");
            }
            if (User.IsInRole("Department head"))
            {
            ViewBag.ActiveTab = "All";
                var loanApplications = _service.GetLoanApplications(true);
            return View(loanApplications);
        }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public ActionResult New()
        {
            var loanApplications = _service
                .GetLoanApplications(a => a.Status == LoanApplicationStatus.New || a.Status == LoanApplicationStatus.InitiallyApproved);
            ViewBag.ActiveTab = "New";
            return View("Index", loanApplications);
        }

        public ActionResult Reviewed()
        {
            var loanApplications = _service
                .GetLoanApplications(a => a.Status == LoanApplicationStatus.Approved || a.Status == LoanApplicationStatus.Rejected);
            ViewBag.ActiveTab = "Reviewed";
            return View("Index", loanApplications);
        }

        public ActionResult OnSecurityReview()
        {
            var loanApplications = _service
                .GetLoanApplications(a => a.Status == LoanApplicationStatus.UnderRiskConsideration);
            ViewBag.ActiveTab = "Security";
            return View("Index", loanApplications);
        }

        public ActionResult OnCommitteeReview()
        {
            var loanApplications = _service
                .GetLoanApplications(a => a.Status == LoanApplicationStatus.UnderCommitteeConsideration);
            ViewBag.ActiveTab = "Committee";
            return View("Index", loanApplications);
        }

        public ActionResult Contracted()
        {
            var loanApplications = _service.GetLoanApplications(a => a.Status == LoanApplicationStatus.Contracted);
            ViewBag.ActiveTab = "Contracted";
            return View("Index", loanApplications);
        }


        //// GET: /Committee/Details/5
        //[Authorize(Roles = "Credit committee")]
        //public ActionResult Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var application = _service.GetLoanApplications(la => la.Id == id).SingleOrDefault();
        //    if (application == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var history = _service.GetHistoryFromNationalBank(application);
        //    if (history == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var customerId =
        //        application.Documents.Single(
        //            d => d.DocType == DocType.Passport && d.TariffDocType == TariffDocType.DebtorPrimary).CustomerId;
        //    var viewModel = new PersonalLoanHistoryViewModel { Id = customerId, Application = application, History = history };
        //    return View(viewModel);
        //}

        public ActionResult Approved()
        {
            var loanApplications = _service
                .GetLoanApplications(a => a.Status == LoanApplicationStatus.Approved);
            ViewBag.ActiveTab = "Approved";
            return View("Index", loanApplications);
        }

        public ActionResult Rejected()
        {
            var loanApplications = _service.GetLoanApplications(a => a.Status == LoanApplicationStatus.Rejected);
            ViewBag.ActiveTab = "Rejected";
            return View("Index", loanApplications);
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return View(loanApplication);
        }

        [AllowAnonymous]
        public ActionResult Create()
        {
            var tariffs = _service.GetTariffs();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.New;
            loanApplication.TimeCreated = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _service.CreateLoanApplication(loanApplication);
                }
                catch (ArgumentException e)
                {
                    var validationResult = e.Data["validationResult"] as Dictionary<string, string>;
                    if (validationResult != null)
                    {
                        foreach (var result in validationResult)
                        {
                            ModelState.AddModelError(result.Key, result.Value);
                        }
                        var tariffList = _service.GetTariffs();
                        ViewBag.TariffId = new SelectList(tariffList, "Id", "Name");
                        return View();
                    }
                }
                return View("Created");
            }

            var tariffs = _service.GetTariffs();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
            return View(loanApplication);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanapplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            var tariffs = _service.GetTariffs();
            ViewBag.Tariff = new SelectList(tariffs, "Id", "Name");
            return View(loanapplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoanApplication loanApplication)
        {
            if (ModelState.IsValid)
            {
                _service.UpsertLoanApplication(loanApplication);
                return RedirectToAction("Index");
            }
            var tariffs = _service.GetTariffs();
            ViewBag.Tariff = new SelectList(tariffs, "Id", "Name");
            return View(loanApplication);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return View(loanApplication);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            _service.DeleteLoanApplicationById(id);
            return RedirectToAction("Index");
        }

        public ActionResult Contract(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Preview", "Loan", new { loanApplicationId = loanApplication.Id});
        }

        public ActionResult Approve(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            _service.ApproveLoanAppication(loanApplication);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(Guid id)
        {
            LoanApplication loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication != null)
            {
                _service.ApproveLoanAppication(loanApplication);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Reject(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            _service.RejectLoanApplication(loanApplication);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(Guid id)
        {
            LoanApplication loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication != null)
            {
                _service.RejectLoanApplication(loanApplication);
            }
            return RedirectToAction("Index");
        }

        public ActionResult SecondStage(Guid id)
        {
            throw new NotImplementedException();
        }

        public ActionResult History(Guid id)
        {
            throw new NotImplementedException();
        }

        public ActionResult SendToSecurity(LoanApplication application)
        {
            _service.SendLoanApplicationToCommittee(application);
            return RedirectToAction("Index");
        }

        public ActionResult SendToCommittee(LoanApplication application)
        {
            _service.SendLoanApplicationToCommittee(application);
            return RedirectToAction("Index");
        }

        public ActionResult Fill(Guid? id)
        {
            if (id == null)
            {
                return View();
            }
            var loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            var selectedTariffId = "";
            if (loanApplication == null)
            {
                return HttpNotFound();
            }

            selectedTariffId = loanApplication.TariffId.ToString();
            var tariffs = _service.GetTariffs();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name", selectedTariffId);
            return View(loanApplication);
        }

        [HttpPost, ActionName("Fill")]
        [ValidateAntiForgeryToken]
        public ActionResult Fill(LoanApplication loanApplication)
        {
            var original = _service.GetLoanApplications(l => l.Id.Equals(loanApplication.Id)).Single();
            var selectedTariffId = "";
            if (original != null)
            {
                loanApplication.TimeCreated = original.TimeCreated;
                selectedTariffId = original.TariffId.ToString();
            }
            if (ModelState.IsValid)
            {
                _service.UpsertLoanApplication(loanApplication);
            }
            var tariffList = _service.GetTariffs();
            ViewBag.TariffId = new SelectList(tariffList, "Id", "Name");
            return View(loanApplication);
        }
    }
}