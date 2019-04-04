using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Ajax_Bootstrap.Models;

namespace MVC_Ajax_Bootstrap.Controllers
{
    public class BlogController : Controller
    {
		BlogEntities ctx = new BlogEntities();
		// GET: Blog

		//Kullanıcı Listeleme
        public ActionResult Index()
        {
			List<Kullanici> kullanicilar = ctx.Kullanicis.ToList();
            return View(kullanicilar);
        }


		public ActionResult KullaniciOlustur()
		{
			return View();
		}


		[HttpPost]
		public bool KullaniciOlustur(Kullanici k1, HttpPostedFileBase k1Foto)
		{
			k1.KullaniciFotoUrl = $"MEDIA\\IMG\\Profil\\{DateTime.Now.ToShortDateString()}_{Guid.NewGuid().ToString().Replace(" - ", "")}_{k1Foto.FileName}";
			k1.KullaniciKayitTarih = DateTime.Now;
			try
			{
				k1Foto.SaveAs(Server.MapPath("~/" + k1.KullaniciFotoUrl));
				ctx.Kullanicis.Add(k1);
				ctx.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		[HttpPost]
		public bool KullaniciSil(int KullaniciID)
		{
			Kullanici sil = ctx.Kullanicis.FirstOrDefault(x => x.KullaniciID == KullaniciID);

			try
			{
				ctx.Kullanicis.Remove(sil);
				ctx.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}



		public ActionResult Edit(int KullaniciID)
		{
			Kullanici k1 = ctx.Kullanicis.FirstOrDefault(k => k.KullaniciID == KullaniciID);
			return View(k1);
		}

		[HttpPost]
		public bool Edit(Kullanici gelenKullanici, HttpPostedFileBase yeniFoto)
		{
			Kullanici k1 = ctx.Kullanicis.First(k => k.KullaniciID == gelenKullanici.KullaniciID);
			try
			{
				if (yeniFoto != null)
				{
					gelenKullanici.KullaniciFotoUrl = $"MEDIA\\IMG\\KullaniciFoto\\{DateTime.Now.ToShortDateString()}_{Guid.NewGuid().ToString().Replace(" - ", "")}_{yeniFoto.FileName}";
					try
					{
						yeniFoto.SaveAs(Server.MapPath("~/" + gelenKullanici.KullaniciFotoUrl));
						//eski fotoyu sil.
						k1.KullaniciFotoUrl = gelenKullanici.KullaniciFotoUrl;

						var kullanicieskiFotoPath = Server.MapPath("..\\" + k1.KullaniciFotoUrl);
						System.IO.File.Delete(kullanicieskiFotoPath);
					}
					catch (Exception)
					{
						return false;
					}
				}

				k1.KullaniciAdSoyad = gelenKullanici.KullaniciAdSoyad;
				k1.KullaniciDogumTarih = gelenKullanici.KullaniciDogumTarih;
				k1.KullaniciFotoUrl = gelenKullanici.KullaniciFotoUrl;
				k1.KullaniciParola = gelenKullanici.KullaniciParola;
				k1.KullaniciEposta = gelenKullanici.KullaniciEposta;
				ctx.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}


		public ActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public bool Login(string txtEmail, string txtParola)
		{
			Kullanici k1 = ctx.Kullanicis.FirstOrDefault(x => x.KullaniciEposta == txtEmail && x.KullaniciParola == txtParola);
			if (k1 != null)
			{
				//Session.Add("KullaniciID", k1.KullaniciID);
				Session["KullaniciID"] = k1.KullaniciID;
				return true;
			}
			else
			{
				ViewBag.SessionHata = "HATALI";
				return false;
				//RedirectToAction("Login");
			}
		}

		public ActionResult Logout()
		{
			Session["KullaniciID"] = null;
			return RedirectToAction("Login");
		}
	}
}