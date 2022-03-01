using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    public class TablesController : Controller
    {
        private SchoolManagement_DBEntities1 db = new SchoolManagement_DBEntities1();

        // GET: Tables
        public async Task<ActionResult> Index()
        {
            var tables = db.Tables.Include(t => t.Course).Include(t => t.Student).Include(t => t.Lecturer);
            return View(await tables.ToListAsync());
        }

        // GET: Tables/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = await db.Tables.FindAsync(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // GET: Tables/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Ttile");
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "LastName");
            ViewBag.LectureID = new SelectList(db.Lecturers, "ID", "First_Name");
            return View();
        }

        // POST: Tables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EnrollmentID,Grade,CourseID,StudentID,LectureID")] Table table)
        {
            if (ModelState.IsValid)
            {
                db.Tables.Add(table);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Ttile", table.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "LastName", table.StudentID);
            ViewBag.LectureID = new SelectList(db.Lecturers, "ID", "First_Name", table.LectureID);
            return View(table);
        }

        [HttpPost]
        public async Task<JsonResult> AddStudent([Bind(Include = "CourseID,StudentID")] Table table) {
            try
            {
                var IsEnrolled = db.Tables.Any(q => q.CourseID == table.CourseID && q.StudentID == table.StudentID) ;// to check if already added

                if (ModelState.IsValid && !IsEnrolled)
                {
                    db.Tables.Add(table);
                    await db.SaveChangesAsync();
                    return Json(new { IsSuccess = true, Message = "Student Added Successfully"},JsonRequestBehavior.AllowGet);
                }
                return Json(new { IsSuccess = false, Message = "Student is Already Added " }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { IsSuccess = false, Message = "System failure: Please Contact Administrator ; " }, JsonRequestBehavior.AllowGet);


            }
        }

        // GET: Tables/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = await db.Tables.FindAsync(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Ttile", table.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "LastName", table.StudentID);
            ViewBag.LectureID = new SelectList(db.Lecturers, "ID", "First_Name", table.LectureID);
            return View(table);
        }

        // POST: Tables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EnrollmentID,Grade,CourseID,StudentID,LectureID")] Table table)
        {
            if (ModelState.IsValid)
            {
                db.Entry(table).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Ttile", table.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "LastName", table.StudentID);
            ViewBag.LectureID = new SelectList(db.Lecturers, "ID", "First_Name", table.LectureID);
            return View(table);
        }

        // GET: Tables/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = await db.Tables.FindAsync(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Table table = await db.Tables.FindAsync(id);
            db.Tables.Remove(table);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetStudents(string term)
        {
            var students = db.Students.Select(q => new
            {
                Name = q.FirstName + " " + q.LastName,
                Id = q.StudentID

            }).Where(q => q.Name.Contains(term)) ;

            return Json(students, JsonRequestBehavior.AllowGet);
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
