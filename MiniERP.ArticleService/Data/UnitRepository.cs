﻿using Microsoft.EntityFrameworkCore;
using MiniERP.ArticleService.Exceptions;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Data
{
    public class UnitRepository : IUnitRepository
    {
        private readonly AppDbContext _context;

        public UnitRepository(AppDbContext context)
        {
            _context = context;
        }
        public void AddUnit(Unit item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _context.Units.Add(item);
        }

        public IEnumerable<Unit> GetAllUnits()
        {
            return _context.Units.ToList();
        }

        public Unit? GetUnitById(int id)
        {
            return _context.Units.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveUnit(Unit item)
        {
            _context.Units.Remove(item);
        }

        public bool SaveChanges()
        {
            try
            {
                return _context.SaveChanges() >= 0;
            }
            catch (DbUpdateException updEx)
            {
                throw new SaveChangesException(typeof(Unit), updEx.Message);
            }
        }
    }
}