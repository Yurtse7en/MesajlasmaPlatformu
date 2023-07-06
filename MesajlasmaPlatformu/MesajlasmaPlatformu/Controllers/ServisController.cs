using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MesajlasmaPlatformu.Models;
using MesajlasmaPlatformu.ViewModel;


namespace MesajlasmaPlatformu.Controllers
{
    public class ServisController : ApiController
    {
        DB03Entities2 db = new DB03Entities2();
        SonucModel sonuc = new SonucModel();
        DateTime currentTime = DateTime.Now;

        #region User
        [HttpGet]
        [Route("api/userliste")]
        public List<UserModel> UserListe()
        {
            List<UserModel> liste = db.User.Select(x => new UserModel()
            {
                userId = x.userId,
                userName = x.userName,
                userEmail = x.userEmail
            }).ToList();
            return liste;
        }

        [HttpGet]
        [Route("api/userbyid/{userId}")]
        public UserModel UserById(int userId)
        {
            UserModel kayit = db.User.Where(s => s.userId == userId).Select(x => new UserModel()
            {
                userId = x.userId,
                userName = x.userName,
                userEmail = x.userEmail,
            }).SingleOrDefault();
            return kayit;
        }

        [HttpPost]
        [Route("api/userEkle")]
        public SonucModel UserEkle(UserModel model)
        {
            if (db.User.Count(c => c.userEmail == model.userEmail) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Girilen Mail Kullanılıyor!";
                return sonuc;

            }
            User yeni = new User();
            yeni.userName = model.userName;
            yeni.userPassword = model.userPassword;
            yeni.userEmail = model.userEmail;
            db.User.Add(yeni);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Yeni Kullanıcı Eklendi";
            return sonuc;
        }

        [HttpPut]
        [Route("api/userduzenle")]
        public SonucModel UserDuzenle(UserModel model)
        {
            User kayit = db.User.Where(s => s.userId == model.userId).FirstOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kullanıcı Bulunamadı!";
                return sonuc;
            }

            kayit.userName = model.userName;
            kayit.userEmail = model.userEmail;

            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Kullanıcı Düzenlendi";

            return sonuc;
        }
        [HttpDelete]
        [Route("api/usersil/{userId}")]
        public SonucModel UserSil(int userId)
        {
            User kayit = db.User.Where(s => s.userId == userId).FirstOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kullanıcı Bulunamadı!";
                return sonuc;
            }

            if (db.GroupMember.Count(c => c.gmuserId == userId) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Gruba Kayıtlı Kullanıcı Silinemez!";
                return sonuc;
            }

            db.User.Remove(kayit);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Kullanıcı Silindi";

            return sonuc;
        }
        #endregion


        #region Mesaj
        [HttpGet]
        [Route("api/mesajliste")]
        public List<MesajModel> mesajListe()
        {
            List<MesajModel> liste = db.Mesaj.Select(x => new MesajModel()
            {
                messageId = x.messageId,
                senderId = x.senderId,
                recipientId = x.recipientId,
                content = x.content,
                mesajtarih = x.mesajtarih,    
            }).ToList();
            return liste;
        }

        [HttpGet]
        [Route("api/mesajbyid/{messageId}")]
        public MesajModel MesajById(int messageId)
        {
            MesajModel kayit = db.Mesaj.Where(s => s.messageId == messageId).Select(x => new MesajModel()

            {
                messageId = x.messageId,
                senderId = x.senderId,
                recipientId = x.recipientId,
                content = x.content,
                mesajtarih = x.mesajtarih,
            }).SingleOrDefault();
            return kayit;
        }

        [HttpPost]
        [Route("api/mesajEkle")]
        public SonucModel MesajEkle(MesajModel model)
        {
            bool kayit = db.Mesaj.Any(s => s.recipientId == model.recipientId);
            bool kayit2 = db.Mesaj.Any(s => s.senderId == model.senderId);

            if (!kayit)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Alıcı Bulunamadı";  
            }

            else if (!kayit2)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Gönderici Bulunamadı";
            }

            else
            {
                Mesaj yeni = new Mesaj();
                yeni.senderId = model.senderId;
                yeni.recipientId = model.recipientId;
                yeni.content = model.content;
                yeni.mesajtarih = currentTime.ToString();
                db.Mesaj.Add(yeni);
                db.SaveChanges();
                sonuc.islem = true;
                sonuc.mesaj = "Mesajınız Gönderildi";
            }
            return sonuc;
        }
        [HttpPost]
        [Route("api/grupMesaj")]
        public SonucModel grupMesaj(MesajModel model)
        {
            bool kayit2 = db.Mesaj.Any(s => s.senderId == model.senderId);

            if (!kayit2)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Gönderici Bulunamadı";
            }
            else
            {
                List<int> recipientIds = db.GroupMember
                .Where(gm => gm.gmgrupId == model.recipientId)
                .Select(gm => gm.gmuserId.Value)
                .ToList();

                foreach (int recipientId in recipientIds)
                {
                    
                    Mesaj yeni = new Mesaj();
                    yeni.senderId = model.senderId;
                    yeni.recipientId = recipientId;
                    yeni.content = model.content;
                    yeni.mesajtarih = currentTime.ToString();
                    db.Mesaj.Add(yeni);
                    
                }

                db.SaveChanges();
                sonuc.islem = true;
                sonuc.mesaj = "Mesajınız Gönderildi";
            }

            return sonuc;
        }

        [HttpPost]
        [Route("api/birdenFazlaMesaj")]
        public SonucModel birdenFazlaMesaj(MesajModel model)
        {
            List<int> recipientIdList = new List<int>() { 3, 8 }; 
        {
            foreach (int recipientId in recipientIdList)
            {
                Mesaj yeni = new Mesaj();
                yeni.senderId = model.senderId;
                yeni.recipientId = recipientId;
                yeni.content = model.content;
                yeni.mesajtarih = currentTime.ToString();
                db.Mesaj.Add(yeni);
            }

            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Mesajınız Gönderildi";
        }

        return sonuc;
        }


        [HttpPut]
        [Route("api/mesajduzenle")]
        public SonucModel MesajDuzenle(MesajModel model)
        {
            Mesaj kayit = db.Mesaj.Where(s => s.messageId == model.messageId).FirstOrDefault();
            bool kayit1 = db.Mesaj.Any(s => s.recipientId == model.recipientId);
            bool kayit2 = db.Mesaj.Any(s => s.senderId == model.senderId);

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Mesajınız Bulunamadı!";
            }
            else if (!kayit1)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Alıcı Bulunamadı";
            }
            else if (!kayit2)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Gönderici Bulunamadı";
            }
            else
            {
                kayit.content = model.content;
                db.SaveChanges();
                sonuc.islem = true;
                sonuc.mesaj = "Mesajınız Düzenlendi";
            }
            return sonuc;
        }
        [HttpDelete]
        [Route("api/mesajsil/{messageId}")]
        public SonucModel MesajSil(int messageId)
        {
            Mesaj kayit = db.Mesaj.Where(s => s.messageId == messageId).FirstOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Mesajınız Bulunamadı!";
                return sonuc;
            }


            db.Mesaj.Remove(kayit);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Mesajınız Silindi";

            return sonuc;
        }
        #endregion


        #region Grup
        [HttpGet]
        [Route("api/grupliste")]
        public List<GrupModel> grupListe()
        {
            List<GrupModel> liste = db.Grup.Select(x => new GrupModel()
            {
                grupId = x.grupId,
                grupAdi = x.grupAdi,
                olusturanId = (int)x.olusturanId,
                tarih = x.tarih,
            }).ToList();
            return liste;
        }

        [HttpGet]
        [Route("api/grupbyid/{grupId}")]
        public GrupModel GrupById(int grupId)
        {
            GrupModel kayit = db.Grup.Where(s => s.grupId == grupId).Select(x => new GrupModel()
            {
                grupId = x.grupId,
                grupAdi = x.grupAdi,
                olusturanId = (int)x.olusturanId,
                tarih = x.tarih,
            }).SingleOrDefault();
            return kayit;
        }

        [HttpPost]
        [Route("api/grupEkle")]
        public SonucModel GrupEkle(GrupModel model)
        {
            bool kayit = db.Grup.Any(s => s.olusturanId == model.olusturanId);

            if (!kayit)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Hatalı ID!";
            }

            else
            { 
                Grup yeni = new Grup();
                yeni.grupId = model.grupId;
                yeni.grupAdi = model.grupAdi;
                yeni.olusturanId = model.olusturanId;
                yeni.tarih = currentTime.ToString();
                db.Grup.Add(yeni);
                db.SaveChanges();
                sonuc.islem = true;
                sonuc.mesaj = "Yeni Grup Eklendi";
            }
            return sonuc;
        }

        [HttpPut]
        [Route("api/grupduzenle")]
        public SonucModel GrupDuzenle(GrupModel model)
        {
            Grup kayit = db.Grup.Where(s => s.grupId == model.grupId).FirstOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Grup Bulunamadı!";
            }

            else
            {
                kayit.grupAdi = model.grupAdi;
                db.SaveChanges();
                sonuc.islem = true;
                sonuc.mesaj = "Grup İsmi Düzenlendi";
            }
            return sonuc;
        }
        [HttpDelete]
        [Route("api/grupsil/{grupId}")]
        public SonucModel GrupSil(int grupId)
        {
            Grup kayit = db.Grup.Where(s => s.grupId == grupId).FirstOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Grup Bulunamadı!";
                return sonuc;
            }


            db.Grup.Remove(kayit);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Grup Silindi";

            return sonuc;
        }
        #endregion

        #region GrupMember
        [HttpGet]
        [Route("api/groupMemberListe")]
        public List<GroupMemberModel> groupMemberListe()
        {
            List<GroupMemberModel> liste = db.GroupMember.Select(x => new GroupMemberModel()
            {
                gmId = x.gmId,
                gmgrupId = x.gmgrupId,
                gmuserId = (int)x.gmuserId,
            }).ToList();
            return liste;
        }

        [HttpGet]
        [Route("api/groupMemberbyGroupId/{gmgrupId}")]
        public List<GroupMemberModel> GroupMemberByGroupId(int gmgrupId)
        {
            List<GroupMemberModel> kayitlar = db.GroupMember
                .Where(gm => gm.gmgrupId == gmgrupId)
                .Join(db.User, gm => gm.gmuserId, user => user.userId, (gm, user) => new GroupMemberModel
                {
                    gmId = gm.gmId,
                    gmgrupId = gm.gmgrupId,
                    gmuserId = (int)gm.gmuserId,
                    userName = user.userName
                })
                .ToList();

            return kayitlar;
        }

        [HttpGet]
        [Route("api/groupMemberbyUserId/{gmuserId}")]
        public List<GroupMemberModel> GroupMemberByUserId(int gmuserId)
        {
            List<GroupMemberModel> kayitlar = db.GroupMember
                .Where(gm => gm.gmuserId == gmuserId)
                .Join(db.Grup, gm => gm.gmgrupId, grup => grup.grupId, (gm, grup) => new GroupMemberModel
                {
                    gmId = gm.gmId,
                    gmuserId = (int)gm.gmuserId,
                    gmgrupId = gm.gmgrupId,
                    grupAdi = grup.grupAdi
                })
                .ToList();

            return kayitlar;
        }


        [HttpPost]
        [Route("api/GroupMemberEkle")]
        public SonucModel GroupMemberEkle(GroupMemberModel model)
        {
            bool kayit = db.GroupMember.Any(s => s.gmgrupId == model.gmgrupId);
            bool kayit2 = db.GroupMember.Any(s => s.gmuserId == model.gmuserId);

            if (!kayit)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Bu isimde bir grup bulunmamakta!";
            }

            else if (!kayit2)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Bu isimde bir kullanıcı bulunmamakta!";
            }

            else
            {
                GroupMember yeni = new GroupMember();
                yeni.gmId = model.gmId;
                yeni.gmgrupId = model.gmgrupId;
                yeni.gmuserId = model.gmuserId;
                db.GroupMember.Add(yeni);
                db.SaveChanges();
                sonuc.islem = true;
                sonuc.mesaj = "Gruba kullanıcı tanımlandı.";
            }
            return sonuc;
        }

        [HttpDelete]
        [Route("api/groupMemberSil/{gmuserId}")]
        public SonucModel groupMemberSil(int gmuserId)
        {
            GroupMember kayit = db.GroupMember.Where(s => s.gmuserId == gmuserId).FirstOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Bu isimde bir kullanıcı bulunamadı!";
                return sonuc;
            }


            db.GroupMember.Remove(kayit);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Kullanıcı Gruptan Çıkarıldı!";

            return sonuc;
        }
        #endregion
    }

}