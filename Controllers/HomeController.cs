using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using userDashboard.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace userDashboard.Models
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet]
        [Route("")]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/signin")]
        public ViewResult Login()
        {
            return View();
        }

        [HttpGet]
        [Route("/register")]
        public ViewResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("/create/user")]
        public RedirectToActionResult Create(User user)
        {
            Console.WriteLine(user.FirstName);
            Console.WriteLine(user.LastName);
            Console.WriteLine(user.Email);
            Console.WriteLine("Password");
            Console.WriteLine(user.Password);
            Console.WriteLine("Confirm Password");
            Console.WriteLine(user.ConfirmPassword);
            if (ModelState.IsValid)
            {
                if(dbContext.Users.Any(user1 => user1.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                }
                User InitialUser = dbContext.Users.FirstOrDefault(u => u.UserId == 1);
                if (InitialUser == null)
                {
                    PasswordHasher <User> AdminHasher = new PasswordHasher<User>();
                    User newAdmin = new User()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Password = AdminHasher.HashPassword(user, user.Password),
                        IsAdmin = true,
                    };
                    dbContext.Add(newAdmin);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("userId", newAdmin.UserId);
                    return RedirectToAction("AdminDashboard");
                    }
                PasswordHasher <User> Hasher = new PasswordHasher<User>();
                User newUser = new User()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = Hasher.HashPassword(user, user.Password),
                    IsAdmin = false,
                };
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("userId", newUser.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                Console.WriteLine("Registration failed!*******");
                return RedirectToAction("Register");
            }
        }

        [HttpGet]
        [Route("/admin")]
        public IActionResult AdminDashboard()
        {
            List <User> allUsers = dbContext.Users.ToList();
            int? session_Id = HttpContext.Session.GetInt32("userId");
            Console.WriteLine(session_Id);
            Console.WriteLine("!!!!!!!*******!!!!!!!*******");
            User userInSession = dbContext.Users.FirstOrDefault(u => u.UserId == session_Id);
            Console.WriteLine(userInSession.IsAdmin);
            Console.WriteLine("!!!!!!!*******!!!!!!!*******");
            if (userInSession.IsAdmin == true)
            {
                return View(allUsers);
            }
            else
            {
                return RedirectToAction("Logout");
            }
        }

        [HttpGet]
        [Route("/dashboard")]
        public ViewResult Dashboard()
        {
            List <User> allUsers = dbContext.Users.ToList();
            return View(allUsers);
        }

        [HttpPost]
        [Route("/session/create")]
        public RedirectToActionResult SignIn(LoginUser usersubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDB = dbContext.Users.FirstOrDefault(u => u.Email == usersubmission.email);
                if (userInDB == null)
                {
                    ModelState.AddModelError("Email", "Invalid email");
                    return RedirectToAction("Login");
                }
                var Hasher = new PasswordHasher<LoginUser>();
                var result = Hasher.VerifyHashedPassword(usersubmission, userInDB.Password, usersubmission.password);
                if (result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid password!");
                    return RedirectToAction("Login");
                }
                int id = userInDB.UserId;
                HttpContext.Session.SetInt32("userId", id);
                if (userInDB.IsAdmin == true)
                {
                    return RedirectToAction("AdminDashboard");
                }
                return RedirectToAction("Dashboard");
            }
            else
            {
                Console.WriteLine("Login failed!*******");
                return RedirectToAction("Login");
            }
        }
        [HttpGet]
        [Route("/signout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/users/new")]
        public IActionResult New()
        {
            int? session_Id = HttpContext.Session.GetInt32("userId");
            User userInSession = dbContext.Users.FirstOrDefault(u => u.UserId == session_Id);
            Console.WriteLine(userInSession.IsAdmin);
            Console.WriteLine("!!!!!!!*******!!!!!!!*******");
            if (userInSession.IsAdmin == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Logout");
            }
        }

        [HttpPost]
        [Route("/admin/create")]
        public RedirectToActionResult CreateUser(User user)
        {
            Console.WriteLine(user.FirstName);
            Console.WriteLine(user.LastName);
            Console.WriteLine(user.Email);
            Console.WriteLine("Password");
            Console.WriteLine(user.Password);
            Console.WriteLine("Confirm Password");
            Console.WriteLine(user.ConfirmPassword);
            Console.WriteLine(user.IsAdmin);
            if (ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                }
                PasswordHasher <User> Hasher = new PasswordHasher<User>();
                User newUser = new User()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = Hasher.HashPassword(user, user.Password),
                    IsAdmin = user.IsAdmin,
                };
                Console.WriteLine(newUser.FirstName);
                Console.WriteLine(newUser.LastName);
                Console.WriteLine(newUser.Email);
                Console.WriteLine("Password");
                Console.WriteLine(newUser.Password);
                Console.WriteLine("Confirm Password");
                Console.WriteLine(user.ConfirmPassword);
                Console.WriteLine(newUser.IsAdmin);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                Console.WriteLine("Failed to create the user created by admin!");
                return RedirectToAction("New");
            }
        }
        [HttpGet]
        [Route("/users/edit/{userId}")]
        public IActionResult AdminEdit(int userId)
        {
            int? session_Id = HttpContext.Session.GetInt32("userId");
            User userInSession = dbContext.Users.FirstOrDefault(u => u.UserId == session_Id);
            if (userInSession.IsAdmin == true)
            {
                User user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                ViewBag.firstName = user.FirstName;
                ViewBag.lastName = user.LastName;
                ViewBag.Email = user.Email;
                return View();
            }
            else
            {
                return RedirectToAction("Logout");
            }
        }

        [HttpPost]
        [Route("/admin/update/{userId}")]
        public IActionResult UserUpdate(int userId, UpdatedUser user)
        {
            int? session_Id = HttpContext.Session.GetInt32("userId");
            User userInSession = dbContext.Users.FirstOrDefault(u => u.UserId == session_Id);
            if (userInSession.IsAdmin == true)
            {
                Console.WriteLine(user.firstName);
                Console.WriteLine(user.lastName);
                Console.WriteLine(user.email);
                Console.WriteLine(user.IsAdmin);
                User selectedUser = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                if (ModelState.IsValid)
                {
                    selectedUser.FirstName = user.firstName;
                    selectedUser.LastName = user.lastName;
                    selectedUser.Email = user.email;
                    selectedUser.IsAdmin = user.IsAdmin;
                    selectedUser.UpdatedAt = DateTime.Now;
                    dbContext.SaveChanges();
                    return RedirectToAction("AdminDashboard");
                }
                else
                {
                    int id = userId;
                    Console.WriteLine("Update by admin failed!********");
                    return RedirectToAction("AdminEdit", new {userId = id});
                }
            }
            return RedirectToAction("Logout");
        }
        [HttpPost]
        [Route("/admin/update/password/{userId}")]
        public RedirectToActionResult AdminUpdatePassword(string password, string confirmPassword, int userId)
        {
            if (password.Length > 7 && confirmPassword.Length > 7)
            {
                if (password == confirmPassword)
                {
                    int? session_Id = HttpContext.Session.GetInt32("userId");
                    User userInSession = dbContext.Users.FirstOrDefault(u => u.UserId == session_Id);
                    User selectedUser = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                    if (selectedUser.UserId != session_Id && userInSession.IsAdmin == false)
                    {
                        Console.WriteLine("You are not qualified to change the password");
                        int id1 = userId;
                        return RedirectToAction("AdminEdit", new {userId = id1});
                    }
                    Console.WriteLine(password);
                    PasswordHasher <User> Hasher = new PasswordHasher<User>();
                    selectedUser.Password = Hasher.HashPassword(selectedUser, password);
                    selectedUser.UpdatedAt = DateTime.Now;
                    Console.WriteLine(selectedUser.Password);
                    dbContext.SaveChanges();
                    return RedirectToAction("AdminDashboard");
                }
                int id = userId;
                Console.WriteLine("Password and password confirmation must match");
                return RedirectToAction("AdminEdit", new {userId = id});
            }
            else
            {
                int id = userId;
                Console.WriteLine("Password must be atleast 8 characters");
                return RedirectToAction("AdminEdit", new {userId = id});
            }
        }
        [HttpGet]
        [Route("/users/edit")]
        public IActionResult Edit()
        {
            int? session_Id = HttpContext.Session.GetInt32("userId");
            User userInSession = dbContext.Users.FirstOrDefault(u => u.UserId == session_Id);
            ViewBag.firstName = userInSession.FirstName;
            ViewBag.lastName = userInSession.LastName;
            ViewBag.email = userInSession.Email;
            ViewBag.isAdmin = userInSession.IsAdmin;
            return View();
        }
        [HttpPost]
        [Route("/update")]
        public RedirectToActionResult Update(UpdatedUser user)
        {
            if (ModelState.IsValid)
            {
                int? session_Id = HttpContext.Session.GetInt32("userId");
                User userInSession = dbContext.Users.FirstOrDefault(u => u.UserId == session_Id);
                if (userInSession != null)
                {
                    userInSession.FirstName = user.firstName;
                    userInSession.LastName = user.lastName;
                    userInSession.Email = user.email;
                    userInSession.UpdatedAt = DateTime.Now;
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    int? session_Id1 = HttpContext.Session.GetInt32("userId");
                    int? id = session_Id1;
                    Console.WriteLine("No user in session!!!!!!!!*******");
                    return RedirectToAction("Edit");
                }
            }
            else
            {
                int? session_Id = HttpContext.Session.GetInt32("userId");
                int? id = session_Id;
                Console.WriteLine("Update failed!!!!!!!!*******");
                return RedirectToAction("Edit");
            }
        }
        [HttpGet]
        [Route("/delete/{userId}")]
        public RedirectToActionResult Delete(int userId)
        {
            User selectedUser = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            dbContext.Users.Remove(selectedUser);
            dbContext.SaveChanges();
            return RedirectToAction("AdminDashboard");
        }
        [HttpPost]
        [Route("/update/description")]
        public RedirectToActionResult Description(string newDescription)
        {
            int? session_Id = HttpContext.Session.GetInt32("userId");
            if (newDescription.Length > 8)
            {
                User userInSession = dbContext.Users.FirstOrDefault(u => u.UserId == session_Id);
                userInSession.Description = newDescription;
                userInSession.UpdatedAt = DateTime.Now;
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                Console.WriteLine("Description must be more than 8 characters!");
                return RedirectToAction("Edit");
            }
        }

        [HttpPost]
        [Route("/update/password")]
        public RedirectToActionResult UpdatePassword(string password, string confirmPassword)
        {
            if (password.Length > 7 && confirmPassword.Length > 7)
            {
                if (password == confirmPassword)
                {
                    int? session_Id = HttpContext.Session.GetInt32("userId");
                    User userInSession = dbContext.Users.FirstOrDefault(u => u.UserId == session_Id);
                    Console.WriteLine(password);
                    PasswordHasher <User> Hasher = new PasswordHasher<User>();
                    userInSession.Password = Hasher.HashPassword(userInSession, password);
                    userInSession.UpdatedAt = DateTime.Now;
                    Console.WriteLine(userInSession.Password);
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    Console.WriteLine("Password and password confirmation must match");
                return RedirectToAction("Edit");
                }
            }
            else
            {
                Console.WriteLine("Password must be atleast 8 characters");
                return RedirectToAction("Edit");
            }
        }

        [HttpGet]
        [Route("/users/show/{userId}")]
        public ViewResult Show(int userId)
        {
            int? session_Id = HttpContext.Session.GetInt32("userId");
            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            ViewBag.firstName = currentUser.FirstName;
            ViewBag.lastName = currentUser.LastName;
            ViewBag.email = currentUser.Email;
            ViewBag.description = currentUser.Description;
            ViewBag.createdAt = currentUser.CreatedAt;
            ViewBag.id = currentUser.UserId;
            ViewBag.IsAdmin = currentUser.IsAdmin;
            List<Message> allMesagesForCurrentUser = dbContext.Messages.Include(m1 => m1.Creator).Include(g => g.ListOfComments).ToList();
            ViewBag.comments= dbContext.Comments.Include(a => a.UserCreator).Include(b => b.MessageCommentBelongsTo).ToList();
            ViewBag.messages = allMesagesForCurrentUser;
            ViewBag.sessionId = session_Id;

            return View("Show");
        }

        [HttpPost]
        [Route("/messages/create/{userId}")]
        public RedirectToActionResult  CreateMessages(int userId, Message message)
        {
            int? session_Id = HttpContext.Session.GetInt32("userId");
            if (message.Content.Length > 4)
            {
                User personCreatingMessage = dbContext.Users.Include(u => u.CreatedMessages).FirstOrDefault(u1 => u1.UserId == session_Id);
                User personBeingCommentedOn = dbContext.Users.FirstOrDefault(u2 => u2.UserId == userId);
                if (personCreatingMessage == null)
                {
                    Console.WriteLine("********Failed to find the id of user trying to create message!*******");
                    return RedirectToAction("Show", new {userId = userId});
                }
                Message newMessage = new Message()
                {
                    Content = message.Content,
                    Creator = personCreatingMessage,
                    // UserBeingMessagedId = userId,
                };
                dbContext.Messages.Add(newMessage);
                dbContext.SaveChanges();
                Message message1 = dbContext.Messages.FirstOrDefault(m => m.Content == message.Content);
                personCreatingMessage.CreatedMessages.Add(message1);
                return RedirectToAction("Show", new {userId = userId});
            }
            else
            {
                Console.WriteLine("********Message must be atleast 4 characters.");
                return RedirectToAction("Show", new {userId = userId});
            }
        }
        
        [HttpPost]
        [Route("/comments/create/{userId}/{messageId}")]
        public RedirectToActionResult CreateComments(int userId, int messageId, Comment comment)
        {
            Console.WriteLine("*******message id!!!!!!!*******");
            Console.WriteLine(messageId.GetType() == typeof(int));
            Console.WriteLine(messageId);
            Console.WriteLine("*******message id!!!!!!!*******");
            Console.WriteLine("*******user id!!!!!!!*******");
            Console.WriteLine(userId.GetType() == typeof(int));
            Console.WriteLine(userId);
            Console.WriteLine("*******user id!!!!!!!*******");
            Console.WriteLine("*******Session id!!!!!!!*******");
            // Console.WriteLine(sessionId1);
            // Console.WriteLine(sessionId1.GetType() == typeof(int));
            Console.WriteLine("*******Session id!!!!!!!*******");
            if (ModelState.IsValid)
            {
                int? sessionId = HttpContext.Session.GetInt32("userId");
                User personCreatingComment = dbContext.Users.Include(z => z.CreatedComments).FirstOrDefault(u1 => u1.UserId == sessionId);
                User personBeingCommentedOn = dbContext.Users.FirstOrDefault(u3 => u3.UserId == userId);
                Message messageCommentBelongTo = dbContext.Messages.Include(x => x.ListOfComments).FirstOrDefault(m1 => m1.MessageId == messageId);
                if (personCreatingComment == null | messageCommentBelongTo == null)
                {
                    Console.WriteLine("*******Failed to find the id of user trying to create message or find Id of message comment belongs too.");
                    return RedirectToAction("Show", new {userId = userId});
                }
                // Random rando = new Random();
                // string randomString = "";
                // char[] chars ="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
                // for (int i = 0; i < rando.Next(0,250); i++)
                // {
                //     randomString = randomString + chars[rando.Next(0, chars.Length)];
                // }
                Console.WriteLine(comment.Content);
                Comment newComment = new Comment()
                {
                    Content = comment.Content,
                    MessageId = messageId,
                    UserBeingCommentedOnId = userId,
                    UserId = personCreatingComment.UserId,
                };
                dbContext.Comments.Add(newComment);
                dbContext.SaveChanges();
                // Console.WriteLine(comment.Content);
                Comment createdComment = dbContext.Comments.FirstOrDefault(f => f.Content == comment.Content);
                Console.WriteLine("*******!!!!!!!*******");
                messageCommentBelongTo.ListOfComments.Add(newComment);
                personCreatingComment.CreatedComments.Add(newComment);
                return RedirectToAction("Show", new {userId = userId});
                // if (personCreatingComment.IsAdmin == true)
                // {
                //     return RedirectToAction("AdminDashboard");
                // }
                // else
                // {
                //     return RedirectToAction("Dashboard");
                // } 
            }
            else
            {
                int? sessionId = HttpContext.Session.GetInt32("userId");
                User personCreatingComment = dbContext.Users.FirstOrDefault(u1 => u1.UserId == sessionId);
                
                if (personCreatingComment.IsAdmin == true)
                {
                    Console.WriteLine("*******Comment must be atleast 4 characters long!");
                    return RedirectToAction("AdminDashboard");
                }
                else
                {
                    Console.WriteLine("*******Comment must be atleast 4 characters long!");
                    return RedirectToAction("Dashboard");
                } 
            }
        }
        [HttpGet]
        [Route("/goHome")]
        public RedirectToActionResult Home()
        {
            int? sessionId = HttpContext.Session.GetInt32("userId");
            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == sessionId);
            if (currentUser.IsAdmin == true)
            {
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                return RedirectToAction("Dashboard");
            }
        }
    }
}