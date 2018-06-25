using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level5Logic : LevelLogic
{
    public GameObject sofaSpawned;
    public GameObject[] tableware;
    public GameObject[] attacheds;
    public GameObject regalInteractable;
    public GameObject regalDestroyed;

    // Use this for initialization
    void Start()
    {
        base.Start();

        attacheds = new GameObject[tableware.Length];
        int i = 0;
        foreach(GameObject go in tableware)
        {
            attacheds[i] = go.GetComponentInChildren<hasBeenAttached>().gameObject;
            i++;
        }


        Level4Logic.turnOnWashingMachine();

        DialogSys.addDialog("AI", "Und da sind wir auch schon!", Mood.PROUD);
        DialogSys.addDialog("AI", "Wie Du siehst naehert sich diese Welt mehr und mehr Deinem eigentlichen Zuhause an.", Mood.HAPPY);
        DialogSys.addDialog("AI", "Ein paar Sachen sind mir da natuerlich noch aufgefallen.", Mood.THINKING);
        DialogSys.addDialog("AI", "Du hast Dich beispielsweise in der ersten Trainingseinheit sichtlich echauffiert, das ganze Geschirr vom Boden zu heben.", Mood.WINKING);
        DialogSys.addDialog("AI", "Damit wir das fuer die Zukunft aendern koennen, habe ich mir die Freiheit genommen, ein Regal aufzustellen.", Mood.PROUD);
        DialogSys.addDialog("AI", "Waerst Du so gut, dies eben einzuraeumen? Ich verspreche auch, das wird das letzte Mal sein.", Mood.OOPS, "cmd_choice1");
        DialogSys.display();


    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        if(progress == 1)
        {
            bool allAttached = true;
            foreach (GameObject go in attacheds)
                if (!go.GetComponent<hasBeenAttached>().beenAttached)
                    allAttached = false;
            if(allAttached)
            {
                cmd_addProgress();
                cmd_3();
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
            cmd_3();
    }
    
    public void cmd_choice1()
    {
        DialogSys.showMultipleChoice("Ich dachte, das haetten wir hinter uns...", "cmd_1a", "In Ordnung, so schwer kann es ja nicht sein.", "cmd_1b", "Schoenes Sofa.", "cmd_1c");
    }

    public void cmd_1a()
    {
        DialogSys.addDialog("Player", "Ich dachte, das haetten wir hinter uns...");
        DialogSys.addDialog("AI", "Vom Boden auf den Tisch, ja. Aber sein wir mal ehrlich.", Mood.NEUTRAL);
        DialogSys.addDialog("AI", "Als zivilisiertes Individuum bewahrst Du dein Geschirr doch nicht auf dem Boden auf, oder?", Mood.WINKING);
        DialogSys.addDialog("Player", "Wieso hast Du es dann nicht von Anfang an dadrin?!");
        DialogSys.addDialog("AI", "Wir muessen doch sicherstellen, dass der Optomat auch der Regal-Be- und Entladevorgang fehlerfrei ausfuehrt.", Mood.PROUD, "cmd_1_followup");
        DialogSys.display();
    }
    public void cmd_1b()
    {
        DialogSys.addDialog("Player", "In Ordnung, so schwer kann es ja nicht sein.");
        DialogSys.addDialog("AI", "Ist es auch nicht. Glaub mir. Das haben schon viele vor Dir geschafft.", Mood.LAUGHING);
        DialogSys.addDialog("AI", "Sehr. Viele.", Mood.LAUGHING, "cmd_1_followup");
        DialogSys.display();
    }
    public void cmd_1c()
    {
        DialogSys.addDialog("Player", "Schoenes Sofa.");
        DialogSys.addDialog("AI", "Du hast wirklich ein Auge fuers Detail!", Mood.HAPPY);
        DialogSys.addDialog("AI", "Vergiss nicht, ich kann alles, was Du auch kannst.", Mood.PROUD);
        DialogSys.addDialog("AI", "Willst Du vielleicht ... noch eins?", Mood.THINKING, "cmd_choice2");
        DialogSys.display();
    }

    public void cmd_choice2()
    {
        DialogSys.showMultipleChoice("Man kann nie genug Sofas haben!", "cmd_2a", "Lass mal sein, hier wird es mir eh schon zu voll", "cmd_2b");
    }
    public void cmd_2a()
    {
        DialogSys.addDialog("Player", "Man kann nie genug Sofas haben!");
        DialogSys.addDialog("AI", "Es erwaermt mein Herz, Menschen gluecklich zu machen.", Mood.LOVING);
        DialogSys.addDialog("AI", "Ich werde es an einem strategisch guenstigen Ort platzieren.", Mood.WINKING);
        DialogSys.addDialog("AI", "Schliesslich hast Du von hier aus die schoenste Aussicht ueberhaupt.", Mood.ASHAMED, "cmd_2_followup");
        DialogSys.display();
    }
    public void cmd_2b()
    {
        DialogSys.addDialog("Player", "Lass mal sein, hier wird es mir eh schon zu voll");
        DialogSys.addDialog("AI", "Du enttaeuschst mich.", Mood.DISAPPOINTED);
        DialogSys.addDialog("AI", "Das werde ich mir merken. So schnell gibt es jedenfalls nichts mehr geschenkt.", Mood.WINKING, "cmd_1_followup");
        DialogSys.display();
    }

    public void cmd_2_followup()
    {
        // spawn 2nd sofa
        sofaSpawned.SetActive(true);
        cmd_1_followup();
    }
    public void cmd_1_followup()
    {
        DialogSys.addDialog("AI", "Jetzt aber los an die Arbeit.", Mood.HAPPY);
        DialogSys.addDialog("AI", "Vergiss nicht - Je schneller Du hier fertig bist, umso schneller ist der Optomat einsatzbereit.", Mood.HAPPY);
        DialogSys.addDialog("AI", "Und dann wartet ein traumhaftes Leben auf Dich. Ohne Sorgen. Ohne Muehen. Ohne Probleme.", Mood.PROUD);
        DialogSys.display();
        cmd_addProgress();
    }

    public void cmd_3()
    {
        //GameObject.Find("BoxshelfModel").GetComponent<Animator>().SetTrigger("Start");
        regalInteractable.SetActive(false);
        regalDestroyed.SetActive(true);
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Physical"))
        {
            if(go.GetComponent<hasBeenAttached>() != null)
            {
                go.GetComponentInChildren<Rigidbody>().AddForce(0, 150, -350);
                go.GetComponentInChildren<Meshinator>().destroyMe();
            }
        }

        DialogSys.addDialog("AI", "Ooooooops...", Mood.OOPS);
        DialogSys.addDialog("Player", "Was ... war ... DAS?");
        DialogSys.addDialog("AI", "Ich wollte den Schrank eigentlich nur ein bisschen umschmeissen.", Mood.OOPS);
        DialogSys.addDialog("AI", "Aber Sie wissen ja. Ich kann meine Kraft so schlecht kontrollieren.", Mood.ANGRY, "cmd_choice4");
        DialogSys.display();
    }

    public void cmd_choice4()
    {
        DialogSys.showMultipleChoice("Warum wolltest Du ihn UMSCHMEISSEN!?", "cmd_4a", "Willst Du mich irgendwie unter Druck setzen??", "cmd_4b", "Ich habe doch gerade alles eingeraeumt...", "cmd_4c");
    }
    public void cmd_4a()
    {
        DialogSys.addDialog("Player", "Warum wolltest Du ihn UMSCHMEISSEN!?");
        DialogSys.addDialog("AI", "Was denken Sie denn?", Mood.SMIRKING);
        DialogSys.addDialog("AI", "An Einraeumdaten habe ich langsam genug. Es wird Zeit fuer die wichtigeren Sachen.", Mood.WINKING);
        DialogSys.addDialog("Player", "Wichtig? Wieso entscheidest Du eigentlich, was wichtig ist!?");
        DialogSys.addDialog("AI", "Ich befolge nur meine Prioritaetsheuristiken. Die Optocorp-Entwicklungsstandards lassen da leider nicht viel Spielraum.", Mood.ASHAMED);
        DialogSys.addDialog("AI", "Und mal ehrlich. Wenn ein paar Sachen falsch eingeraeumt sind, ist das doch nicht so wichtig.", Mood.NEUTRAL);
        DialogSys.addDialog("AI", "Aber stellen Sie sich vor, wenn so viele Scherben im Haus herumliegen. Und ihre kleine Tochter spielt damit.", Mood.SPOOKED);
        DialogSys.addDialog("Player", "Lass meine Tochter da raus! Was willst Du jetzt eigentlich von mir?!", "cmd_4_followup");
        DialogSys.display();
    }
    public void cmd_4b()
    {
        DialogSys.addDialog("Player", "Willst Du mich irgendwie unter Druck setzen??");
        DialogSys.addDialog("AI", "Aber keinesfalls. Vergessen Sie nicht. Das alles spielt sich nur in Ihrem Kopf ab.", Mood.SAD);
        DialogSys.addDialog("Player", "Ich hab mich aber ganz schön real erschrocken. Und ich wette, Schmerzen fühlen sich auch ziemlich real an...");
        DialogSys.addDialog("AI", "Dann passen Sie besser gut auf Sich auf.", Mood.WINKING, "cmd_4_followup");
        DialogSys.display();
    }
    public void cmd_4c()
    {
        DialogSys.addDialog("Player", "Ich habe doch gerade alles eingeraeumt...");
        DialogSys.addDialog("AI", "Korrekt. Der Einraeumvorgang war zufriedenstellend.", Mood.PROUD, "cmd_4_followup");
        DialogSys.display();
    }

    public void cmd_4_followup()
    {
        DialogSys.addDialog("AI", "Was nun folgt ist das Scherbenbeseitigungsprogramm.", Mood.OOPS);
        DialogSys.addDialog("AI", "Benutzen Sie die Ihnen zur Verfügung gestellten Gegenstände, um die Scherben in dem Muelleimer zu entsorgen.", Mood.WINKING);
        DialogSys.addDialog("AI", "Zu Ihrer eigenen Sicherheit haben wir das Aufheben mit blossen Haenden deaktiviert.", Mood.HAPPY);
        DialogSys.addDialog("AI", "Wir wollen doch nicht, dass Ihnen etwas zustoesst.", Mood.OOPS, "cmd_5");
        DialogSys.display();
    }

    public void cmd_5()
    {
        pm = new ProgressMinigame(120, progressGO);
        runProgressMinigame();
    }

    public void onWashingLoadEnter()
    {

    }
}
