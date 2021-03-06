﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CsvHelper;

using RightpointLabs.Pourcast.Domain.Models;
using RightpointLabs.Pourcast.Application.Orchestrators.Abstract;
using RightpointLabs.Pourcast.Web.Areas.Admin.Models;

namespace RightpointLabs.Pourcast.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class BeerController : Controller
    {
        static BeerController()
        {
            AutoMapper.Mapper.CreateMap<Beer, CreateBeerViewModel>();
            AutoMapper.Mapper.CreateMap<CreateBeerViewModel, Beer>();
            AutoMapper.Mapper.CreateMap<Beer, BeerViewModel>();
            AutoMapper.Mapper.CreateMap<BeerViewModel, Beer>();
            AutoMapper.Mapper.CreateMap<Beer, EditBeerViewModel>();
            AutoMapper.Mapper.CreateMap<EditBeerViewModel, Beer>();

        }
        private readonly IBeerOrchestrator _beerOrchestrator;
        private readonly IBreweryOrchestrator _breweryOrchestrator;
        private readonly IStyleOrchestrator _styleOrchestrator;

        public BeerController(IBeerOrchestrator beerOrchestrator, IBreweryOrchestrator breweryOrchestrator, IStyleOrchestrator styleOrchestrator)
        {
            if (beerOrchestrator == null) throw new ArgumentNullException("beerOrchestrator");
            if (null == breweryOrchestrator) throw new ArgumentNullException("breweryOrchestrator");
            if(null == styleOrchestrator) throw new ArgumentNullException("styleOrchestrator");
            _beerOrchestrator = beerOrchestrator;
            _breweryOrchestrator = breweryOrchestrator;
            _styleOrchestrator = styleOrchestrator;
        }

        //
        // GET: /Admin/Beer/
        public ActionResult Index()
        {
            var vm = new BeerIndexViewModel()
            {
                Beers =
                    _beerOrchestrator.GetBeers()
                        .Select(b => new SelectListItem() {Text = b.Name, Value = b.Id})
                        .OrderBy(i => i.Text)
                        .ToList(),
                Breweries =
                    _breweryOrchestrator.GetBreweries()
                        .Select(b => new SelectListItem() {Text = b.Name, Value = b.Id})
                        .OrderBy(i => i.Text)
                        .ToList()
            };
            return View(vm);
        }

        //
        // GET: /Admin/Beer/Details/My-Beer-Name
        public ActionResult Details(string id)
        {
            var beer = _beerOrchestrator.GetById(id);

            return View(AutoMapper.Mapper.Map<Beer, BeerViewModel>(beer));
        }

        //
        // GET: /Admin/Beer/Create
        public ActionResult Create(string breweryId)
        {
            var brewery = _breweryOrchestrator.GetById(breweryId);
            var styles = _styleOrchestrator.GetStyles();
            if (null != brewery)
            {
                var vm = new CreateBeerViewModel(styles) {BreweryId = brewery.Id, BreweryName = brewery.Name};
                return View(vm);
            }

            ModelState.AddModelError("Brewery", "Brewery with that id does not exist.");
            return View(new CreateBeerViewModel(){BreweryId = breweryId});
        }

        //
        // POST: /Admin/Beer/Create
        [HttpPost]
        public ActionResult Create(CreateBeerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = _beerOrchestrator.GetByBrewery(model.BreweryId);
            if (existing.Any(b => b.Name == model.Name))
            {
                ModelState.AddModelError("BeerName", "A beer with that name already exists for this brewery.");
                return View(model);
            }

            string id = _beerOrchestrator.CreateBeer(model.Name, model.ABV, model.BAScore, model.StyleId, model.BreweryId);

            return RedirectToAction("Details", "Beer", new { id = id});
        }

        //
        // GET: /Admin/Beer/Edit/5
        public ActionResult Edit(string id)
        {
            var beer = _beerOrchestrator.GetById(id);
            var styles = _styleOrchestrator.GetStyles();
            if (null == beer)
            {
                ViewBag.Error = "No beer with that id exists";
                return View();
            }
            var brewery = _breweryOrchestrator.GetById(beer.BreweryId);
            var model = AutoMapper.Mapper.Map<Beer, EditBeerViewModel>(beer);
            model.Styles =
                styles.Select(s => new SelectListItem() {Text = s.Name, Value = s.Id, Selected = (s.Id == beer.StyleId)}).OrderBy(i => i.Text).ToList();

            model.BreweryName = brewery.Name;
            return View(model);
        }

        //
        // POST: /Admin/Beer/Edit/5
        [HttpPost]
        public ActionResult Edit(EditBeerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _beerOrchestrator.Save(AutoMapper.Mapper.Map<EditBeerViewModel, Beer>(model));
            return RedirectToAction("Details", "Beer", new {id = model.Id});
        }

        public ActionResult Import(string breweryId)
        {
            var model = new ImportBeerViewModel();
            var brewery = _breweryOrchestrator.GetById(breweryId);
            if (null == brewery)
            {
                ViewBag.Error = "Brewery with that id does not exist.";
                return View(model);
            }

            model.BreweryName = brewery.Name;
            return View(model);
        }

        [HttpPost]
        public ActionResult Import(ImportBeerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.File.ContentLength <= 0) return View(model);

            if (model.File.FileName.Contains("csv"))
            {
                throw new NotImplementedException();
                try
                {
                    var beers = new List<ImportBeerModel>();
                    using (var reader = new StreamReader(model.File.InputStream))
                    {
                        using (var csv = new CsvReader(reader))
                        {
                            beers = csv.GetRecords<ImportBeerModel>().ToList();
                        }
                    }

                    beers.ForEach(b =>
                    {
                        var existing = _beerOrchestrator.GetByBrewery(model.BreweryId);
                        if (false == existing.Any(beer => beer.Name == b.Name))
                        {
                            //_beerOrchestrator.CreateBeer(b.Name, b.ABV?? 0, b.BAScore?? 0, b.Style, string.Empty, string.Empty, model.BreweryId);
                        }
                    });

                    return RedirectToAction("Details", "Brewery", new { id = model.BreweryId });
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "File not formatted correctly.";
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "File is not a csv.";
            }
            return View();
        }

        //
        // GET: /Admin/Beer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Admin/Beer/Delete/5
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
