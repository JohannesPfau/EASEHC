using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level0Logic : LevelLogic {

	// Use this for initialization
	void Start ()
    {
        base.Start();

        DialogSys.addDialog("Daughter", "Papa, Papa, der Optomat ist da!");
        DialogSys.addDialog("Player", "Oha.. das Paket ist ja größer als ich dachte.");
        DialogSys.addDialog("Player", "Wisst ihr was ... Der Papa schließt das Ding jetzt erstmal an und ihr bastelt aus dem Karton ein schönes Spielhaus.");
        DialogSys.addDialog("Daughter", "Jaaaa!");
        DialogSys.addDialog("Player", "Na bitte.");
        DialogSys.addDialog("Player", "Also, so schwer kann das ja nicht sein. Den Akku lade ich schonmal auf.");
        DialogSys.addDialog("Player", "Dann weg mit dem ganzen Verpackungsmaterial...");
        DialogSys.addDialog("Player", "Du meine Güte. Das ist ja ein richtiger Brocken.");
        DialogSys.addDialog("Player", "Okay, Anleitung ... Ja, ja, das ist sowieso was für Dummköpfe.");
        DialogSys.addDialog("Player", "Also hier. Letzter Schritt. Indi..viduelle Konfiguration.");
        DialogSys.addDialog("Player", "\"Durch individuelle Nutzermodellkonfiguration wird das bestmögliche Haushaltserlebnis gewährleistet. Bitte starten sie nun die Sitzung, indem sie den beigelegten BCI-Transmitter in ihrer rechten Ohrmuschel verankern.\"");
        DialogSys.addDialog("Player", "Na dann wollen wir mal. Ade, lästige Haushaltsarbeiten. Hallo, Optomat 9000!");
        DialogSys.addDialog("Player", ". . .");
        DialogSys.addDialog("Player", "Hm.");
        DialogSys.addDialog("Player", "Geht doch tiefer rein als ich dachte.");
        DialogSys.addDialog("Player", ". . . Au!","cmd_toLevel1");
        DialogSys.display();
    }
	
    public void cmd_toLevel1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }
}
