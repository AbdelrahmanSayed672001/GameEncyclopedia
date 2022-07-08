using GamesWebSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;




namespace GamesWebSite.Controllers
{
    public class HomeController : Controller
    {
        private Context db;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _appEnvironment;

        public HomeController(Context context, Microsoft.AspNetCore.Hosting.IHostingEnvironment appEnvironment)
        {
            db = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Reg()
        {
            return View();
        }


        public IActionResult Home(int? id)
        {
            TempData["UserId"] = id;
            if (id != null)
            {
                foreach (var item in db.users)
                {
                    if (item.Id == id)
                    {
                        TempData["UserId"] = item.Id;
                        TempData["UserUsername"] = item.Name;
                        TempData["UserPassword"] = item.Password;
                        TempData["UserPhone"] = "+2"+item.PhoneNumber;
                        TempData["UserAge"] = item.Age;
                        TempData["img"] = item.imgSrc;
                        TempData["frontImg"] = item.frontSrc;
                        TempData["backImg"] = item.backSrc;
                    }
                }
            }

            return View();
        }





        [HttpPost]
        public async Task<IActionResult> UserRegister(User user)
        {
            if (CheckRegister(user, db))
            {
                if (user.img != null)
                {
                    string Folder = "image/";
                    Folder += Guid.NewGuid().ToString() + "_" + user.img.FileName;
                    string serverFolder = Path.Combine(_appEnvironment.WebRootPath, Folder);
                    await user.img.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                    user.imgSrc = Folder;
                }
                TempData["img"] = user.imgSrc;


                if (user.frontImg != null)
                {
                    string Folder = "frontImage/";
                    Folder += Guid.NewGuid().ToString() + "_" + user.frontImg.FileName;
                    string serverFolder = Path.Combine(_appEnvironment.WebRootPath, Folder);
                    await user.frontImg.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                    user.frontSrc = Folder;
                }
                TempData["frontImg"] = user.frontSrc;

                if (user.backImg != null)
                {
                    string Folder = "backImage/";
                    Folder += Guid.NewGuid().ToString() + "_" + user.backImg.FileName;
                    string serverFolder = Path.Combine(_appEnvironment.WebRootPath, Folder);
                    await user.backImg.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                    user.backSrc = Folder;
                }
                TempData["backImg"] = user.backSrc;

                addUser(user, db);
                
                return RedirectToAction("VerificationCode", new { id = user.Id });
            }
            else
            {
                TempData["Register Error"] = user.Name + " is exits before";
                return RedirectToAction("Reg");
            }

        }
        public Boolean CheckRegister(User user, Context db)
        {
            if (!db.users.Any(x => x.Name.Equals(user.Name)))
            {
                return true;
            }
            else
                return false;
        }
        public void addUser(User user, Context db)
        {
            db.users.Add(user);
            db.SaveChanges();
        }


        [HttpPost]
        public IActionResult UserLogin(User model)
        {
            if (CheckUserName(model))
            {
                if (CheckUserPassword(model))
                {
                    RenderUserData(model);
                    return RedirectToAction("Home", new { id = TempData["UserId"] });
                }
                else
                {
                    TempData["LoginError"] = "Username or password is incorrect";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Error"] = "Something wrong was happened";
                return RedirectToAction("Index", "Home");
            }

        }


        public Boolean CheckUserName(User model)
        {
            if (db.users.Any(x => x.Name.Equals(model.Name)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean CheckUserPassword(User model)
        {
            if (db.users.Any(x => x.Password.Equals(model.Password)))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void RenderUserData(User model)
        {
            int currentId;

            foreach (var item in db.users)
            {
                if (item.Name.Equals(model.Name))
                {
                    currentId = item.Id;
                    TempData["UserId"] = item.Id;
                    TempData["UserUsername"] = item.Name;
                    TempData["UserPassword"] = item.Password;
                    TempData["UserPhone"] = "+2" + item.PhoneNumber;
                    TempData["UserAge"] = item.Age;
                    TempData["img"] = item.imgSrc;
                    TempData["frontImg"] = item.frontSrc;
                    TempData["backImg"] = item.backSrc;
                }
            }
        }

        public IActionResult VerificationCode(int? id)
        {
            TempData["UserId"] = id;
            if (id != null)
            {
                foreach (var item in db.users)
                {
                    if (item.Id == id)
                    {
                        TempData["UserId"] = item.Id;
                        TempData["UserUsername"] = item.Name;
                        TempData["UserPassword"] = item.Password;
                        TempData["UserPhone"] = "+2" + item.PhoneNumber;
                        TempData["UserAge"] = item.Age;
                        TempData["img"] = item.imgSrc;
                        TempData["frontImg"] = item.frontSrc;
                        TempData["backImg"] = item.backSrc;
                    }
                }
            }
            var acountSid = "AC0ce23373c9b1eb412360af04042a826e";
            var authToken = "095f411c52217eed1b7031d4967954b7";
            TwilioClient.Init(acountSid, authToken);

            var sender = new PhoneNumber("+19783916598");
            var recieverNumber = Convert.ToString(TempData["UserPhone"]);
            var reciever = new PhoneNumber(recieverNumber);
            TempData["Random"] = RandomOTP();

            var message = MessageResource.Create(
                    to: reciever,
                    from: sender,
                    body: "Your Verification code is: " + TempData["Random"]
                );


            return View();
        }


        public IActionResult SendOTP(OTP otp)
        {
            
            otp.UserId = Convert.ToInt32(TempData["UserId"]);
            var rand = Convert.ToString(TempData["Random"]);
            if (otp.VerifyCode != rand)
            {
                db.oTPs.Add(otp);
                db.SaveChanges();
                TempData["CorrectCode"] = "Welcome To our website";
                return RedirectToAction("Home", new { id = TempData["UserId"] } );
            }
            else
            {
                TempData["IncorrectCode"] = "The OTP you entered is incorrect";
                return RedirectToAction("VerificationCode", new { id = otp.UserId });
            }
            


        }

        public int RandomOTP()
        {
            var randomNumber = new Random().Next(10000, 99999);
            return randomNumber;
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}