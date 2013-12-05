﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain.Models.Loans;
using Domain.Repositories;
using Infrastructure;
using Microsoft.Practices.Unity;
using Application.LoanProcessing;
using Application;

namespace Presentation.Controllers
{
    public class TariffsController : BaseController
    {
        private ProcessingService _service { get; set; }

        public TariffsController()
        {
            _service = new ProcessingService();
        }

        public ActionResult Index()
        {
            var tariffs = _service.GetTariffs(t => true);
            return View(tariffs);
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tariff tariff = _service.GetTariffs(t => t.Id.Equals(id)).Single();
            if (tariff == null)
            {
                return HttpNotFound();
            }
            return View(tariff);
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: /Tariffs/Create
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tariff tariff)
        {
            if (ModelState.IsValid)
            {
                tariff.Id = Guid.NewGuid();
                tariff.CreationDate = DateTime.UtcNow;
                _service.UpsertTariff(tariff);
                return RedirectToAction("Index");
            }

            return View(tariff);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tariff tariff = _service.GetTariffs(t => t.Id.Equals(id)).Single();
            if (tariff == null)
            {
                return HttpNotFound();
            }
            return View(tariff);
        }

        // POST: /Tariffs/Edit/5
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tariff tariff)
        {
            if (ModelState.IsValid)
            {
                _service.UpsertTariff(tariff);
                return RedirectToAction("Index");
            }
            return View(tariff);
        }

        // GET: /Tariffs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tariff tariff = _service.GetTariffs(t => t.Id.Equals(id)).Single();
            if (tariff == null)
            {
                return HttpNotFound();
            }
            return View(tariff);
        }

        // POST: /Tariffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            _service.DeleteTariffById(id);
            return RedirectToAction("Index");
        }
    }
}
