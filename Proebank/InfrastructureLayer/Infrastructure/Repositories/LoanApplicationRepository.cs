﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Loans;

namespace Infrastructure.Repositories
{
    // TODO: don't create datacontext every time and try use IQueryable instead of IList
    public class LoanApplicationRepository : ILoanApplicationRepository
    {
        public LoanApplication Get(Func<LoanApplication, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.LoanApplications
                    .AsQueryable()
                    .First(filter);
            }
        }

        public IList<LoanApplication> GetAll()
        {
            using (var ctx = new DataContext())
            {
                return ctx.LoanApplications.ToList();
            }
        }

        public IList<LoanApplication> GetAll(Func<LoanApplication, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.LoanApplications
                    .AsQueryable()
                    .Where(filter)
                    .ToList();
            }
        }

        public void SaveOrUpdate(params LoanApplication[] entities)
        {
            using (var ctx = new DataContext())
            {
                ctx.LoanApplications.AddOrUpdate(entities);
                ctx.SaveChanges();
            }
        }

        public LoanApplication Delete(LoanApplication entity)
        {
            using (var ctx = new DataContext())
            {
                var removedApplication = ctx.LoanApplications.Remove(entity);
                ctx.SaveChanges();
                return removedApplication;
            }
        }
    }
}
