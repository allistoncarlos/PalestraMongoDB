using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PalestraMongoDB.Domain;
using PalestraMongoDB.Model.Disciplina;
using PalestraMongoDB.Repository;

namespace PalestraMongoDB.Controllers
{
	public class DisciplinaController : Controller
	{
		private IConnectionStringConfiguration connectionStringConfiguration;

		public DisciplinaController(IConnectionStringConfiguration connectionStringConfiguration)
		{
			this.connectionStringConfiguration = connectionStringConfiguration;
		}

		public async Task<IActionResult> Index()
		{
			var repository = new Repository<Disciplina>(connectionStringConfiguration);

			var domains = await repository.Load();
			var models = domains.Select(domain => ToModel(domain));

			return View(models.AsEnumerable());
		}

		public async Task<IActionResult> Edit(string id = null)
		{
			var model = new DisciplinaModel();

			var repository = new Repository<Disciplina>(connectionStringConfiguration);

			if (!string.IsNullOrEmpty(id))
				model = ToModel(await repository.Get(id));

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(DisciplinaModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var repository = new Repository<Disciplina>(connectionStringConfiguration);

			if (string.IsNullOrEmpty(model.Id))
				await repository.Save(ToDomain(model));
			else
				await repository.Update(ToDomain(model));

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Delete(string id)
		{
			var repository = new Repository<Disciplina>(connectionStringConfiguration);

			await repository.Delete(id);

			return RedirectToAction("Index");
		}

		private DisciplinaModel ToModel(Disciplina domain)
		{
			return new DisciplinaModel
			{
				Id = domain.Id.ToString(),
				Nome = domain.Nome,
				Duracao = domain.Duracao
			};
		}

		private Disciplina ToDomain(DisciplinaModel model)
		{
			return new Disciplina
			{
				Id = !string.IsNullOrEmpty(model.Id) ? ObjectId.Parse(model.Id) : ObjectId.GenerateNewId(DateTime.Now),
				Nome = model.Nome,
				Duracao = model.Duracao
			};
		}
	}
}