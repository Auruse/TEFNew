using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEF.Models;

namespace TEF.Controllers
{
    public class TestAccRecordsController : Controller
    {
        private Tests db = new Tests();
        
       
        // GET: TestAccRecords
        public ActionResult Index()
        {
            
            return View(db.TestAccRecords);
        }
        [HttpGet]
        public ActionResult RecordRnd(int? id, int subj )
        {
            ViewBag.Score = 0;
            ViewBag.MaxQ = 1;
            ViewBag.Subject = subj<1 ? 1 : subj;
            ViewBag.CurrQ = 0;
            ViewBag.AnsweredCorrectly = 0;
            if (id.HasValue || id<= db.TestAccRecords.Count())
            {

                //                TestAccRecord testAccRecord = db.TestAccRecords.Find(1);
                var subjKeys = db.TestsDescs.Find(subj).KeyCode;

                var q = db.TestAccRecords.Where(x => x.SubjectId == subj).Select(x => x.Id).ToArray();
                string ts = String.Join(",",q);
                ViewBag.SubjectId = ts;
                ViewBag.MaxQ = q.Count();
               TestAccRecord testAccRecord = db.TestAccRecords.Find(q[0]);
                return View(testAccRecord);
               
            }
            

            return View();
        }
        [HttpPost]
        public ActionResult RecordRnd(FormCollection collection)
        {
            int Score = Convert.ToInt16(collection["Score"]);
            int AnsweredCorrectly = Convert.ToInt16(collection["AnsweredCorrectly"]);
            ViewBag.Score = Score;
            ViewBag.AnsweredCorrectly = AnsweredCorrectly;
            ViewBag.MaxQ = Convert.ToInt16(collection["MaxQ"]);
            ViewBag.Subject = collection["Subject"];
            ViewBag.SubjectId = collection["SubjectId"];
            ViewBag.CurrQ = Convert.ToInt16(collection["CurrQ"]);
            int currq = Convert.ToInt16(collection["CurrQ"]);
            var ts = collection["SubjectId"];
            var showAll = collection["exampleRadios"];
            var ans_n = 1;
            var resans = 0;
            
            if (currq+1 <= ViewBag.MaxQ)
            {
                int id = Convert.ToInt16(ts.ToString().Split(',')[currq]);
                foreach (var i_ans in showAll.Split(','))
                {
                    if (i_ans == "true") resans = ans_n;
                    ans_n += 1;
                }
                if (id <= db.TestAccRecords.Count())
                {
                    TestAccRecord testAccRecord = db.TestAccRecords.Find(id < 1 ? 1 : id);
                    if (Convert.ToInt16(testAccRecord.CorrectAnswer) == resans)
                    {
                        ViewBag.Score += 1;
                        ViewBag.AnsweredCorrectly += 1;
                    }


                    id += 1;
                    if (id <= db.TestAccRecords.Count() && id<= ViewBag.MaxQ)
                    {
                        testAccRecord = db.TestAccRecords.Find(id);
                        ViewBag.CurrQ = id - 1;
                        return View(testAccRecord);
                    }
                    else
                    {
                      View("Report");
                    }
                       
                }
            }
            return View("Report");
        }
        public ActionResult Report()
        {
            
            return View();
        }
        // GET: TestAccRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestAccRecord testAccRecord = db.TestAccRecords.Find(id);
            if (testAccRecord == null)
            {
                return HttpNotFound();
            }
            return View(testAccRecord);
        }

        // GET: TestAccRecords/Create
        public ActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(db.TestAccRecords, "Id", "Desc");
            return View();
        }

        // POST: TestAccRecords/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Dt,Ct,Desc,Answers,CorrectAnswer,SubjectId")] TestAccRecord testAccRecord)
        {
            if (ModelState.IsValid)
            {
                db.TestAccRecords.Add(testAccRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.SubjectId = new SelectList(db.TestsDescs, "Id", "Description", testAccRecord.SubjectId);
            return View(testAccRecord);
        }

        // GET: TestAccRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestAccRecord testAccRecord = db.TestAccRecords.Find(id);
            if (testAccRecord == null)
            {
                return HttpNotFound();
            }
            //ViewBag.SubjectId = new SelectList(db.TestsDescs, "Id", "Description", testAccRecord.SubjectId);
            return View(testAccRecord);
        }

        // POST: TestAccRecords/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Dt,Ct,Desc,Answers,CorrectAnswer,SubjectId")] TestAccRecord testAccRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testAccRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.SubjectId = new SelectList(db.TestsDescs, "Id", "Description", testAccRecord.SubjectId);
            return View(testAccRecord);
        }

        // GET: TestAccRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestAccRecord testAccRecord = db.TestAccRecords.Find(id);
            if (testAccRecord == null)
            {
                return HttpNotFound();
            }
            return View(testAccRecord);
        }

        // POST: TestAccRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestAccRecord testAccRecord = db.TestAccRecords.Find(id);
            db.TestAccRecords.Remove(testAccRecord);
            db.SaveChanges();
            return RedirectToAction("Index");
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
