using System;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.HabboHotel.NotifTop
{
    public class NotifTopManager
    {
        private readonly List<string> NotifMessage;

        public NotifTopManager()
        {
            this.NotifMessage = new List<string>();
        }

        public void Init()
        {
            this.NotifMessage.Clear();

            //Charger les notif depuis une table Mysql
            this.NotifMessage.Add("Participe aux animations pour remporter des points gamers, des ExtraBox et des badgeBox");
            this.NotifMessage.Add("Double-clique sur le mobi TV-Vidéos pour regarder ou ajouter une vidéo YouTube");
            this.NotifMessage.Add("Remporte des win-win en faisant les missions sur Wibbo");
            this.NotifMessage.Add("Le saviez-vous ? Il y a plus de 700 000 inscrits sur Wibbo en 8 ans");
            this.NotifMessage.Add("Clique sur le bouton Aide en haut à droite de ton écran pour lancer un appel à l'aide");
            this.NotifMessage.Add("Tu peux modifier ton pseudo toutes les 24 heures ou tout le temps si tu es Premium");
            this.NotifMessage.Add("Tu peux te prendre en photo et y mettre un filtre tout ça sur Wibbo en cliquant sur l'appareil photo en bas de l'écran");
            this.NotifMessage.Add("Vous pouvez échanger vos badge grâce au mobi \"Troc de Badge\" disponible dans le Point Shop du catalogue");
            this.NotifMessage.Add("Le saviez-vous ? Une animation automatique est lancée toutes les 30 minutes sans animation si plus de 200 joueurs sont connectés");
            this.NotifMessage.Add("Rejoins nos réseaux sociaux pour être le premier au courant de l'actualité de Wibbo");
            this.NotifMessage.Add("Vous pouvez jouer au pierre-feuille-ciseaux en utilisant la commande :janken pseudo");
            this.NotifMessage.Add("Le saviez-vous ? Wibbo a vu le jour pour la première fois le 2 mai 2011 Nous sommes en 2019, Wibbo fêtera son 8éme anniversaire");
            this.NotifMessage.Add("Le 25 octobre 2016 à 16h19 précisemment, Wibbo a eu son record de connectés qui est de 1572");
            this.NotifMessage.Add("Le saviez-vous ? Plus de 900 millions de mobis ont déjà été acheté sur Wibbo");
        }

        public List<string> GetAllMessages()
        {
           return this.NotifMessage.OrderBy(a => Guid.NewGuid()).ToList();
        }
    }
}
