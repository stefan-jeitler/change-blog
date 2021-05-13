using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters
{
    public abstract class BasePresenter
    {
        public ActionResult Response { get; protected set; } = new StatusCodeResult(500);
    }
}
