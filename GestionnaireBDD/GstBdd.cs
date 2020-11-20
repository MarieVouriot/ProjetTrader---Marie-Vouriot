using MySql.Data.MySqlClient;
using System;
using MetierTrader;
using System.Collections.Generic;

namespace GestionnaireBDD
{
    public class GstBdd
    {
        private MySqlConnection cnx;
        private MySqlCommand cmd;
        private MySqlDataReader dr;

        // Constructeur
        public GstBdd()
        {
            string chaine = "Server=localhost;Database=bourse;Uid=root;Pwd=";
            cnx = new MySqlConnection(chaine);
            cnx.Open();
        }

        public List<Trader> getAllTraders()
        {
            List<Trader> lesTraders = new List<Trader>();
            cmd = new MySqlCommand("select idTrader, nomTrader from trader", cnx);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Trader unTrader = new Trader(Convert.ToInt16(dr[0].ToString()), dr[1].ToString());
                lesTraders.Add(unTrader);
            }
            dr.Close();
            return lesTraders;
        }
        public List<ActionPerso> getAllActionsByTrader(int numTrader)
        {
            List<ActionPerso> lesActionsPersos = new List<ActionPerso>();
            cmd = new MySqlCommand("select idAction, nomAction, prixAchat, quantite from action act " +
                "INNER JOIN acheter a on act.idAction = a.numAction INNER JOIN trader t on a.numTrader = t.idTrader where t.idTrader = " + numTrader, cnx);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ActionPerso uneActionPerso = new ActionPerso(Convert.ToInt16(dr[0].ToString()), dr[1].ToString(), Convert.ToDouble(dr[2].ToString()), Convert.ToInt16(dr[3].ToString()));
                lesActionsPersos.Add(uneActionPerso);
            }
            dr.Close();
            return lesActionsPersos;
        }

        public List<MetierTrader.Action> getAllActionsNonPossedees(int numTrader)
        {
            List<MetierTrader.Action> lesActionsNonPossedees = new List<MetierTrader.Action>();
            cmd = new MySqlCommand("SELECT idAction, nomAction from action " +
                "where nomAction not in (select nomAction from action act inner join acheter a on act.idAction = a.numAction where numTrader =" + numTrader +")", cnx);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                MetierTrader.Action uneAction = new MetierTrader.Action(Convert.ToInt16(dr[0].ToString()), dr[1].ToString());
                lesActionsNonPossedees.Add(uneAction);
            }
            dr.Close();
            return lesActionsNonPossedees;
        }

        public void SupprimerActionAcheter(int numAction, int numTrader)
        {
            cmd = new MySqlCommand("delete from acheter where numAction =" + numAction + "and numTrader = " + numTrader, cnx);
            cmd.ExecuteNonQuery();
        }

        public void UpdateQuantite(int numAction, int numTrader, int quantite)
        {
            cmd = new MySqlCommand("insert into acheter values(" + numAction + ", " + numTrader + ","+ quantite +")", cnx);
            cmd.ExecuteNonQuery();
        }

        public double getCoursReel(int numAction)
        {
            double coursReel;
            cmd = new MySqlCommand("SELECT coursReel from action where idAction = " + numAction, cnx);
            dr = cmd.ExecuteReader();
            dr.Read();
            coursReel = Convert.ToDouble(dr[0].ToString());
            dr.Close();
            return coursReel;
        }
        public void AcheterAction(int numAction, int numTrader, double prix, int quantite)
        {
            cmd = new MySqlCommand("insert into acheter values(" + numAction + ", " + numTrader + ","+ prix + "," + quantite + ")", cnx);
            cmd.ExecuteNonQuery();


        }
        public double getTotalPortefeuille(int numTrader)
        {
            double total;
            cmd = new MySqlCommand("select sum(prixAchat * quantite) from acheter where numTrader ="+ numTrader, cnx);
            dr = cmd.ExecuteReader();
            dr.Read();
            total = Convert.ToDouble(dr[0].ToString());
            dr.Close();
            return total;
        }
    }
}
