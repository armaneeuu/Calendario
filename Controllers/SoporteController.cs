using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Calendario.Models;
using Microsoft.EntityFrameworkCore;
using Calendario.Models.Data;

namespace Calendario.Controllers
{
    public class SoporteController :Controller 
    {
        private readonly CalendarioContext _context;

        public SoporteController(CalendarioContext context)
        {
            _context = context;
        }
    }
}