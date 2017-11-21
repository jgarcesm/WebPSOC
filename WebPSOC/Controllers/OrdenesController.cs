using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPSOC.Models;

namespace WebPSOC.Controllers
{
    public class OrdenesController : Controller
    {
        //public ActionResult Cargar(int id)
        //{
        //    if (id == 1)
        //        {
        //        ViewBag.Message = "hola mundo";

        //    }
        //    else
        //    {
        //        ViewBag.Message = "chao mundo";

        //    }
        //    return View();
        //}
        public ActionResult Cargar()
        {
            ClsCargar CargarOrdenCompra = new ClsCargar();
            CargarOrdenCompra.CargarArchivos();

            return View();
        }

        // GET: Ordenes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Ordenes/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Ordenes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ordenes/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Ordenes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Ordenes/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Ordenes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Ordenes/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
