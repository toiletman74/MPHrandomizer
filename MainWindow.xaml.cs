using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Runtime.Serialization.Formatters;
using System.Collections.ObjectModel;

namespace MPHrandomizer
{
    static class FilePath
    {
        public static string pathEntityFldr;
        public static string fileName;
    }


    public partial class MainWindow : Window
    {
        byte[] valueIn = new byte[] {};
        int location;
        int address;
        int Planet;
        int itemsToPlace;
        int missilesToPlace;
        int etankToPlace;
        int UAEToPlace;
        int beamsToPlace;
        int artifactsToPlace;
        int[] beamToPlace = new int[7];
        int[] locationHasItem = new int[59];
        int[] locationExcluded = new int[59];
        int[] beamLocation = new int[7];
        int[] starterLocationExcluded = new int[16];
        int[] planetHasItems = new int[5];
        int randomnum;
        bool hasRolled;
        bool hasRolledBeam;
        bool hasRolledItem;
        bool rollingBeams;
        int seed;
        byte artifactID = 0x00;
        byte artifactModel = 0x00;
        byte[] linkedEntity = { 0xFF, 0xFF };
        Random rnd = new Random();
        class EntityLocation
        {
            public string LocationName;
            public string FileName;
            public ushort Offset;//These in the comments are what the artifact equivilants are called.
            public byte Enabled; //Active
            public byte HasBase; //HasBase
            //AlwaysEnabled
            public short NotifyEntityId; //Message1Target on these two I'm just assuming, I haven't tried it yet.
            public int CollectedMessage; //Message1 I need to test by putting an artifact in faultline where the imperialist usually is.
            public short Message2Target;
            public int Message2;
            public short Message3Target;
            public int Message3;

            public EntityLocation(string locationName, string fileName, ushort offset, byte enabled, byte hasBase, short notifyEntityId, int collectedMessage, short message2Target, int message2, short message3Target, int message3)
            {
                LocationName = locationName;
                FileName = fileName;
                Offset = offset;
                Enabled = enabled;
                HasBase = hasBase;
                NotifyEntityId = notifyEntityId;
                CollectedMessage = collectedMessage;
                Message2Target = message2Target;
                Message2 = message2;
                Message3Target = message3Target;
                Message3 = message3;
            }
        }
            Dictionary<int, EntityLocation> EntityLocationDict = new Dictionary<int, EntityLocation>()
            {
                //Location name, File name, Offset, Enabled, HasBase, NotifyEntityId, CollectedMessage, Message2Target, Message2, Message3Target, Message3
                {1 , new EntityLocation("AAlinosGateway", "\\Unit1_Land_Ent.bin", 2024, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {2 , new EntityLocation("AEchoHallEtank", "\\Unit1_C0_Ent.bin", 3400, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {3 , new EntityLocation("AEchoHallArtifact", "\\Unit1_C0_Ent.bin", 1920, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {4 , new EntityLocation("AHighGroundMissile", "\\unit1_RM1_Ent.bin", 15872, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {5 , new EntityLocation("AHighGroundArtifact", "\\unit1_RM1_Ent.bin", 5976, 0x01, 0x00, 17, 16, 56, 33, 94, 9)},
                {6 , new EntityLocation("AElderPassage", "\\unit1_RM6_Ent.bin", 1368, 0x01, 0x00, 40, 9, 1, 9, 0, 0)},
                {7 , new EntityLocation("AMagmaDrop", "\\Unit1_C4_Ent.bin", 3264, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {8 , new EntityLocation("ACrashSite", "\\Unit1_C3_Ent.bin", 992, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {9 , new EntityLocation("AAlinosPerch", "\\unit1_RM2_ent.bin", 4228, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {10 , new EntityLocation("ACouncilChamberEtank", "\\unit1_rm3_Ent.bin", 4004, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {11 , new EntityLocation("ACouncilChamberArtifact", "\\unit1_rm3_Ent.bin", 4076, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {12 , new EntityLocation("ACouncilChamberMagmaul", "\\unit1_rm3_Ent.bin", 4148, 0x00, 0x00, 0, 0, 0, 0, 0, 0)},
                {13 , new EntityLocation("AProcessorCore", "\\unit1_rm5_Ent.bin", 4888, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {14 , new EntityLocation("APistonCave", "\\Unit1_C5_Ent.bin", 17196, 0x01, 0x00, 36, 16, 35, 16, 0, 0)},
                {15 , new EntityLocation("CACelestialGateway", "\\unit2_Land_Ent.bin", 4328, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {16 , new EntityLocation("CADataShrine01Artifact", "\\unit2_RM1_Ent.bin", 1660, 0x01, 0x00, 21, 18, 0, 0, 0, 0)},
                {17 , new EntityLocation("CADataShrine01Etank", "\\unit2_RM1_Ent.bin", 8280, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {18 , new EntityLocation("CADataShrine02VoltDriver", "\\unit2_RM2_Ent.bin", 4800, 0x01, 0x01, 46, 9, 0, 0, 0, 0)},
                {19 , new EntityLocation("CADataShrine02Missile", "\\unit2_RM2_Ent.bin", 5168, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {20 , new EntityLocation("CADataShrine02UAE", "\\unit2_RM2_Ent.bin", 9624, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {21 , new EntityLocation("CADataShrine03", "\\unit2_RM3_Ent.bin", 1612, 0x01, 0x00, 22, 18, 0, 0, 0, 0)},
                {22 , new EntityLocation("CASynergyCore", "\\unit2_C4_Ent.bin", 1764, 0x01, 0x00, 22, 9, 0, 0, 0, 0)},
                {23 , new EntityLocation("CATransferLock", "\\Unit2_RM4_Ent.bin", 17316, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {24 , new EntityLocation("CADockingBayArtifact", "\\unit2_RM8_Ent.bin", 4112, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {25 , new EntityLocation("CADockingBayUAE", "\\unit2_RM8_Ent.bin", 7280, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {26 , new EntityLocation("CAIncubationVault01", "\\Unit2_RM5_Ent.bin", 1336, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {27 , new EntityLocation("CAIncubationVault02", "\\Unit2_RM6_Ent.bin", 1124, 0x01, 0x01, 4, 18, 0, 0, 0, 0)},
                {28 , new EntityLocation("CAIncubationVault03", "\\Unit2_RM7_Ent.bin", 944, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {29 , new EntityLocation("CANewArrivalRegistrationArtifact", "\\Unit2_C7_Ent.bin", 7276, 0x01, 0x00, 20, 18, 0, 0, 0, 0)},
                {30 , new EntityLocation("CANewArrivalRegistrationEtank", "\\Unit2_C7_Ent.bin", 11172, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {31 , new EntityLocation("VDOWeaponsComplexArtifact1", "\\Unit3_RM1_Ent.bin", 5572, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {32 , new EntityLocation("VDOWeaponsComplexArtifact2", "\\Unit3_RM1_Ent.bin", 2228, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {33 , new EntityLocation("VDOCortexCPUBattleHammer", "\\Unit3_C2_Ent.bin", 2080, 0x01, 0x01, 19, 9, 0, 0, 0, 0)},
                {34 , new EntityLocation("VDOCortexCPUMissile", "\\Unit3_C2_Ent.bin", 5828, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {35 , new EntityLocation("VDOCompressionChamberArtifact", "\\unit3_rm4_Ent.bin", 3496, 0x01, 0x01, 55, 18, 0, 0, 0, 0)},
                {36 , new EntityLocation("VDOCompressionChamberUAE", "\\unit3_rm4_Ent.bin", 6368, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {37 , new EntityLocation("VDOStasisBunkerArtifact1", "\\Unit3_RM3_Ent.bin", 2432, 0x01, 0x00, 17, 18, 0, 0, 0, 0)},
                {38 , new EntityLocation("VDOStasisBunkerArtifact2", "\\Unit3_RM3_Ent.bin", 7352, 0x01, 0x00, 36, 18, 0, 0, 0, 0)},
                {39 , new EntityLocation("VDOStasisBunkerUAE", "\\Unit3_RM3_Ent.bin", 19804, 0x01, 0x01, 17, 18, 0, 0, 0, 0)},
                {40 , new EntityLocation("VDOFuelStackArtifact", "\\Unit3_RM2_Ent.bin", 8128, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {41 , new EntityLocation("VDOFuelStackMissile", "\\Unit3_RM2_Ent.bin", 17768, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {42 , new EntityLocation("ARCSicTransitEtank", "\\unit4_rm3_Ent.bin", 5068, 0x01, 0x00, 9, 16, 0, 0, 0, 0)}, //TODO find a better way to fix the problem of having two messages that need to be sent out from one item entity
                {43 , new EntityLocation("ARCSicTransitArtifact", "\\unit4_rm3_Ent.bin", 6844, 0x01, 0x01, 6, 16, 9, 16, 0, 0)}, //I fixed this in a ghetto way, I took the second message that this is supposed to send and put it on the other item in the same room.
                {44 , new EntityLocation("ARCIceHiveArtifact", "\\Unit4_RM1_Ent.bin", 5596, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {45 , new EntityLocation("ARCIceHiveJudicator", "\\Unit4_RM1_Ent.bin", 10232, 0x01, 0x00, 205, 9, 0, 0, 0, 0)},
                {46 , new EntityLocation("ARCIceHiveUAE1", "\\Unit4_RM1_Ent.bin", 11116, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {47 , new EntityLocation("ARCIceHiveMissile", "\\Unit4_RM1_Ent.bin", 24512, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {48 , new EntityLocation("ARCIceHiveUAE2", "\\Unit4_RM1_Ent.bin", 52688, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {49 , new EntityLocation("ARCFrostLabyrinthArtifact", "\\unit4_C0_Ent.bin", 2552, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {50 , new EntityLocation("ARCFrostLabyrinthEtank", "\\unit4_C0_Ent.bin", 8720, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {51 , new EntityLocation("ARCFaultLineImperialist", "\\Unit4_RM5_Ent.bin", 4288, 0x00, 0x00, 69, 18, 0, 0, 0, 0)},
                {52 , new EntityLocation("ARCFaultLineArtifact", "\\Unit4_RM5_Ent.bin", 4360, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {53 , new EntityLocation("ARCSanctorusArtifact", "\\unit4_rm4_Ent.bin", 2256, 0x01, 0x00, 38, 18, 0, 0, 0, 0)},
                {54 , new EntityLocation("ARCSanctorusUAE", "\\unit4_rm4_Ent.bin", 12176, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {55 , new EntityLocation("ARCDripMoat", "\\unit4_C1_Ent.bin", 23796, 0x01, 0x00, 0, 0, 0, 0, 0, 0)},
                {56 , new EntityLocation("ARCSubterraneanArtifact", "\\Unit4_RM2_Ent.bin", 1836, 0x01, 0x00, 56, 18, 58, 18, 0, 0)},
                {57 , new EntityLocation("ARCSubterraneanMissile", "\\Unit4_RM2_Ent.bin", 7804, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
                {58 , new EntityLocation("OGoreaPeek", "\\Gorea_Peek_Ent.bin", 268, 0x01, 0x01, 0, 0, 0, 0, 0, 0)}
            };

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void fileSelect_click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = System.Windows.Forms.Application.StartupPath;
            //fbd.SelectedPath = "C:\\Users\\Lenka\\Desktop\\Other stuff\\Projects\\MPH moding tools\\MphRead-0.22.0.0-win\\files\\AMHE1\\levels\\entities";
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FilePath.pathEntityFldr = fbd.SelectedPath;
            }
        }

        private void Randomize_click(object sender, RoutedEventArgs e)
        {
            textblock.Text = "Randomizing...";
            if (seed_txtbox.Text != "")
            {
                seed = Int32.Parse(seed_txtbox.Text);
            }
            else
            {
                Random rnd = new Random();
                seed = rnd.Next();
            }
            itemsToPlace = 52;
            missilesToPlace = 9;
            etankToPlace = 7;
            UAEToPlace = 12;
            beamsToPlace = 6;
            artifactsToPlace = 24;
            for (int i = 0; i <= beamsToPlace; i++)
            {
                beamToPlace[i] = 0;
            }
            for (int i = 0; i <= itemsToPlace + beamsToPlace; i++)
            {
                locationHasItem[i] = 0;
            }
            //if (spoiler_chkbox.IsChecked ?? true)
            //{
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\SpoilerLog.txt"))
                {
                    File.Delete(System.Windows.Forms.Application.StartupPath + "\\SpoilerLog.txt");
                }
            //}
            string spoilerLogPath = System.Windows.Forms.Application.StartupPath + "\\SpoilerLog.txt";
            string logEntry = "Seed: " + Convert.ToString(seed);
            //if (spoiler_chkbox.IsChecked ?? true)
            //{
                using (StreamWriter spoilerLog = new StreamWriter(spoilerLogPath, true))
                {
                spoilerLog.WriteLine("MPHrando v0.2.1");
                spoilerLog.WriteLine(logEntry);
                }
            //}
            //this is the sic transit magmaul barrier patch
            //TODO add a patch to make the door in front of slech v4 a magmaul door and the door to slench v3 to be a battlehammer door to prevent softlocks
            //TODO add the planet patch to here if possible
            //TODO patch the key frames for fault line and the room with the lava golem to send the setactive message with the true parameter to the item location
            string fullPath = FilePath.pathEntityFldr + "\\unit4_rm3_Ent.bin";
            FileStream ws = new FileStream(fullPath, FileMode.Open, FileAccess.Write);
            StreamWriter writeStream = new StreamWriter(ws);
            writeStream.BaseStream.Seek(20316, SeekOrigin.Current);
            writeStream.BaseStream.Write(new byte[] { 0x00 }, 0, 1);
            //fullPath = FilePath.pathEntityFldr + "\\unit1_RM1_Ent.bin";
            //writeStream.BaseStream.Seek(27116, SeekOrigin.Begin);
            //writeStream.BaseStream.Write(new byte[] { 0x02 }, 0, 1);
            writeStream.Close();
            RollBeams();
            textblock.Text = "Done!";
        }

        public void RollItems()
        {
            while (itemsToPlace != 0)
            {
                hasRolled = false;
                if (hasRolledItem == false)
                {
                randomnum = RandomNumberGenerator(3);
                }
                else
                { randomnum++; }
                switch (randomnum)
                {
                    case 1:
                        //place e-tank
                        if (etankToPlace > 0)
                        {
                            itemsToPlace--;
                            etankToPlace--;
                            RollLocation(4);
                        }
                        else { hasRolledItem = true; }
                        break;
                    case 2:
                        //place missile expansion
                        if (missilesToPlace > 0)
                        {
                            itemsToPlace--;
                            missilesToPlace--;
                            RollLocation(6);
                        }
                        else { hasRolledItem = true; }
                        break;
                    case 3:
                        //place UAE
                        if (UAEToPlace > 0)
                        {
                            itemsToPlace--;
                            UAEToPlace--;
                            RollLocation(18);
                        }
                        else { hasRolledItem = true; }
                        break;
                    default:
                        randomnum = 0;
                        break;
                }
            }
        }

        public void RollArtifacts()
        {
            while (artifactsToPlace != 0)
            {
                hasRolled = false;
                if (hasRolledItem == false)
                {
                    randomnum = RandomNumberGenerator(4);
                }
                else
                { randomnum++; }
                //place artifact
                itemsToPlace--;
                artifactsToPlace--;
                RollLocation(19);
            }
            RollItems();
        }

        public void RollBeams()
        {
            int ran = 0;
            while (beamsToPlace > 0)
            {
                if (rollingBeams != true)
                {
                    rollingBeams = true;
                }
                hasRolled = false;
                if (hasRolledBeam == false)
                {
                    ran = RandomNumberGenerator(6);
                    if (beamsToPlace == 5)
                    {
                        LocationExclude();
                    }
                }
                else
                { ran++; }
                switch (ran)
                {
                    case 1:
                        //voltdriver
                        if (beamToPlace[1] != 1)
                        {
                            RollLocation(5);
                            beamToPlace[1] = 1;
                            beamsToPlace--;
                            ExcludePlanets(ran,location);
                        }
                        else { hasRolledBeam = true; }
                        break;
                case 2:
                        //battlehammer
                        if (beamToPlace[2] != 1)
                        {
                            RollLocation(7);
                            beamToPlace[2] = 1;
                            beamsToPlace--;
                            ExcludePlanets(ran, location);
                        }
                        else { hasRolledBeam = true; }
                        break;
                case 3:
                        //imperialist
                        if (beamToPlace[3] != 1)
                        {
                            RollLocation(8);
                            beamToPlace[3] = 1;
                            beamsToPlace--;
                            ExcludePlanets(ran, location);
                        }
                        else { hasRolledBeam = true; }
                        break;
                case 4:
                        //judicator
                        if (beamToPlace[4] != 1)
                        {
                            RollLocation(9);
                            beamToPlace[4] = 1;
                            beamsToPlace--;
                            ExcludePlanets(ran, location);
                        }
                        else { hasRolledBeam = true; }
                        break;
                case 5:
                        //magmaul
                        if (beamToPlace[5] != 1)
                        {
                            RollLocation(10);
                            beamToPlace[5] = 1;
                            beamsToPlace--;
                            ExcludePlanets(ran, location);
                        }
                        else { hasRolledBeam = true; }
                        break;
                case 6:
                        //shockcoil
                        if (beamToPlace[6] != 1)
                        {
                            RollLocation(11);
                            beamToPlace[6] = 1;
                            beamsToPlace--;
                            ExcludePlanets(ran, location);
                        }
                        else { hasRolledBeam = true; }
                        break;
                    default:
                        ran = 0;
                        break;
                }
            }
            if (beamsToPlace == 0)
            {
                FixExclusions();
                rollingBeams = false;
                RollArtifacts();
            }
        }

        public void RollLocation(int item)
        {
            if (hasRolled == false)
            {
                location = RandomNumberGenerator(58);
                if (locationHasItem[5] != 1 && artifactsToPlace == 1)
                {
                    location = 5; //this should make high ground always get an artifact in the artifact location.
                } //it's just here because I haven't done the trigger thing yet. pretty ghetto fix
            }
            else
            {
                location++;
            }
            switch (location)
            {
                case 1:
                    //A Alinos Gateway
                    if (locationHasItem[1] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 2:
                    //A Echo Hall E tank
                    if (locationHasItem[2] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 3:
                    //A Echo Hall Artifact
                    if (locationHasItem[3] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 4:
                    //A High Ground Missile
                    if (locationHasItem[4] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 5:
                    //A High Ground Artifact
                    if (locationHasItem[5] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 6:
                    //A Elder Passage
                    if (locationHasItem[6] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 7:
                    //A Magma Drop
                    if (locationHasItem[7] != 1 && (beamToPlace[4] == 1 || MagmaDroptrickcheckbox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 8:
                    //A Crash Site
                    if (locationHasItem[8] != 1 && (beamToPlace[4] == 1 || Crashsitetrickcheckbox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 9:
                    //A Alinos Perch
                    if (locationHasItem[9] != 1 && beamToPlace[4] == 1 && beamToPlace[5] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 10:
                    //A Council Chamber E tank
                    if (locationHasItem[10] != 1 && beamToPlace[4] == 1 && beamToPlace[5] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 11:
                    //A Council Chamber Artifact
                    if (locationHasItem[11] != 1 && beamToPlace[4] == 1 && beamToPlace[5] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 12:
                    //A Council Chamber Magmaul
                    if (locationHasItem[12] != 1 && beamToPlace[4] == 1 && beamToPlace[5] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 13:
                    //A Processor Core
                    if (locationHasItem[13] != 1 && beamToPlace[4] == 1 && beamToPlace[5] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 14:
                    //A Piston Cave
                    if (locationHasItem[14] != 1 && beamToPlace[4] == 1 && beamToPlace[5] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 15:
                    //CA Celestial Gateway
                    if (locationHasItem[15] != 1 && beamToPlace[2] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 16:
                    //CA Data Shrine 01 Artifact
                    if (locationHasItem[16] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 17:
                    //CA Data Shrine 01 E tank
                    if (locationHasItem[17] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 18:
                    //CA Data Shrine 02 Volt Driver
                    if (locationHasItem[18] != 1 && (beamToPlace[2] == 1 || datashrine02trickcheckbox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 19:
                    //CA Data Shrine 02 Missile
                    if (locationHasItem[19] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 20:
                    //CA Data Shrine 02 UAE
                    if (locationHasItem[20] != 1 && (beamToPlace[2] == 1 || datashrine02trickcheckbox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 21:
                    //CA Data Shrine 03
                    if (locationHasItem[21] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 22:
                    //CA Synergy Core
                    if (locationHasItem[22] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 23:
                    //CA Transfer Lock
                    if (locationHasItem[23] != 1 && (beamToPlace[1] == 1 || DeepCAtrickCheckBox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 24:
                    //CA Docking Bay Artifact
                    if (locationHasItem[24] != 1 && (beamToPlace[1] == 1 || DeepCAtrickCheckBox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 25:
                    //CA Docking Bay UAE
                    if (locationHasItem[25] != 1 && (beamToPlace[1] == 1 || DeepCAtrickCheckBox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 26:
                    //CA Incubation Vault 01
                    if (locationHasItem[26] != 1 && (beamToPlace[1] == 1 || DeepCAtrickCheckBox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 27:
                    //CA Incubation Vault 02
                    if (locationHasItem[27] != 1 && (beamToPlace[1] == 1 || DeepCAtrickCheckBox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 28:
                    //CA Incubation Vault 03
                    if (locationHasItem[28] != 1 && (beamToPlace[1] == 1 || DeepCAtrickCheckBox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 29:
                    //CA New Arrival Registration Artifact
                    if (locationHasItem[29] != 1 && ((beamToPlace[1] == 1 && beamToPlace[6] == 1) || (beamToPlace[6] == 1 && DeepCAtrickCheckBox.IsChecked == true)))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 30:
                    //CA New Arrival Registration E tank
                    if (locationHasItem[30] != 1 && ((beamToPlace[1] == 1 && beamToPlace[6] == 1) || (beamToPlace[6] == 1 && DeepCAtrickCheckBox.IsChecked == true)))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 31:
                    //VDO Weapons Complex Artifact 1
                    if (locationHasItem[31] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 32:
                    //VDO Weapons Complex Artifact 2
                    if (locationHasItem[32] != 1 && beamToPlace[2] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 33:
                    //VDO Cortex CPU Battle Hammer
                    if (locationHasItem[33] != 1 && (beamToPlace[2] == 1 || CortexCPUtrickCheckBox.IsChecked == true))
                    {
                        linkedEntity[0] = 0x15;
                        linkedEntity[1] = 0x00;
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 34:
                    //VDO Cortex CPU Missile
                    if (locationHasItem[34] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 35:
                    //VDO Compression Chamber Artifact
                    if (locationHasItem[35] != 1 && beamToPlace[2] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 36:
                    //VDO Compression Chamber UAE
                    if (locationHasItem[36] != 1 && beamToPlace[2] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 37:
                    //VDO Stasis Bunker Artifact 1
                    if (locationHasItem[37] != 1 && beamToPlace[2] == 1 && beamToPlace[3] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 38:
                    //VDO Stasis Bunker Artifact 2
                    if (locationHasItem[38] != 1 && beamToPlace[2] == 1 && beamToPlace[3] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 39:
                    //VDO Stasis Bunker UAE
                    if (locationHasItem[39] != 1 && beamToPlace[2] == 1 && beamToPlace[3] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 40:
                    //VDO Fuel Stack Artifact
                    if (locationHasItem[40] != 1 && beamToPlace[2] == 1 && beamToPlace[3] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 41:
                    //VDO Fuel Stack Missile
                    if (locationHasItem[41] != 1 && beamToPlace[2] == 1 && beamToPlace[3] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 42:
                    //Arc Sic Transit E tank
                    if (locationHasItem[42] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 43:
                    //Arc Sic Transit Artifact
                    if (locationHasItem[43] != 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 44:
                    //Arc Ice Hive Artifact
                    if (locationHasItem[44] != 1 && (beamToPlace[4] == 1 || IcehiveCubbietrickCheckBox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 45:
                    //Arc Ice Hive Judicator
                    if (locationHasItem[45] != 1 && beamToPlace[4] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 46:
                    //Arc Ice Hive UAE 1
                    if (locationHasItem[46] != 1 && beamToPlace[4] == 1)
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 47:
                    //Arc Ice Hive Missile
                    if (locationHasItem[47] != 1 && (beamToPlace[4] == 1 || IcehivemissiletrickCheckBox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 48:
                    //Arc Ice Hive UAE 2
                    if (locationHasItem[48] != 1 && (beamToPlace[4] == 1 || IcehiveCubbietrickCheckBox.IsChecked == true))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 49:
                    //Arc Frost Labyrinth Artifact
                    if (locationHasItem[49] != 1 && (beamToPlace[4] == 1 || beamToPlace[5] == 1 && beamToPlace[3] == 1))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 50:
                    //Arc Frost Labyrinth E tank
                    if (locationHasItem[50] != 1 && (beamToPlace[4] == 1 || beamToPlace[5] == 1 && beamToPlace[3] == 1))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 51:
                    //Arc Fault Line Imperialist
                    if (locationHasItem[51] != 1 && (beamToPlace[4] == 1 && beamToPlace[3] == 1 || beamToPlace[5] == 1))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 52:
                    //Arc Fault Line Artifact
                    if (locationHasItem[52] != 1 && (beamToPlace[4] == 1 && beamToPlace[3] == 1 || beamToPlace[5] == 1))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 53:
                    //Arc Sanctorus Artifact
                    if (locationHasItem[53] != 1 && beamToPlace[3] == 1 && (beamToPlace[4] == 1 || beamToPlace[5] == 1))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 54:
                    //Arc Sanctorus UAE
                    if (locationHasItem[54] != 1 && beamToPlace[3] == 1 && (beamToPlace[4] == 1 || beamToPlace[5] == 1))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 55:
                    //Arc Drip Moat
                    if (locationHasItem[55] != 1 && beamToPlace[3] == 1 && (beamToPlace[4] == 1 || beamToPlace[5] == 1))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 56:
                    //Arc Subterranean Artifact
                    if (locationHasItem[56] != 1 && beamToPlace[3] == 1 && (beamToPlace[4] == 1 || beamToPlace[5] == 1))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 57:
                    //Arc Subterranean Missile
                    if (locationHasItem[57] != 1 && beamToPlace[3] == 1 && (beamToPlace[4] == 1 || beamToPlace[5] == 1))
                    {
                        locationHasItem[location] = 1;
                        PlaceItem(item, location);
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 58:
                    //O Gorea Peek
                    if (locationHasItem[58] != 1 && beamToPlace[1] == 1 && beamToPlace[2] == 1 && beamToPlace[3] == 1 && beamToPlace[4] == 1 && beamToPlace[5] == 1 && beamToPlace[6] == 1)
                    {
                        if (item != 19)
                        {
                            locationHasItem[location] = 1;
                            PlaceItem(item, location);
                        }
                        else
                        {
                            hasRolled = true;
                            RollLocation(item);
                        }
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                default:
                    location = 0;
                    RollLocation(item);
                    break;
            }
        }
        

        public int RandomNumberGenerator(int maxnum)
        {
            Random rnd = new Random(seed);
            int ran = rnd.Next(1, maxnum);
            seed = rnd.Next();
            return ran;
        }

        public void PlaceItem(int item, int location)
        {
            //Location name, File name, Offset, Enabled, HasBase, NotifyEntityId, CollectedMessage, Message2Target, Message2, Message3Target, Message3
            //{ "ASubterraneanMissile", new EntityLocation("\\Unit4_RM2_Ent.bin", 7804, 0x01, 0x01, 0, 0, 0, 0, 0, 0)},
            string spoilerItem = "";
            string fullPath = FilePath.pathEntityFldr + EntityLocationDict[location].FileName;
            string spoilerLogPath = System.Windows.Forms.Application.StartupPath + "\\SpoilerLog.txt";
            switch (item)
            {
                //TODO add cases for all the artifacts

                //EnergyTank
                case 4:
                    spoilerItem = EntityLocationDict[location].LocationName + ": Energy Tank";
                    break;
                //VoltDriver
                case 5:
                    spoilerItem = EntityLocationDict[location].LocationName + ": Volt Driver";
                    break;
                //MissileExpansion
                case 6:
                    spoilerItem = EntityLocationDict[location].LocationName + ": Missile Expansion";
                    break;
                //Battlehammer
                case 7:
                    spoilerItem = EntityLocationDict[location].LocationName + ": Battlehammer";
                    break;
                //Imperialist
                case 8:
                    spoilerItem = EntityLocationDict[location].LocationName + ": Imperialist";
                    break;
                //Judicator
                case 9:
                    spoilerItem = EntityLocationDict[location].LocationName + ": Judicator";
                    break;
                //Magmaul
                case 10:
                    spoilerItem = EntityLocationDict[location].LocationName + ": Magmaul";
                    break;
                //ShockCoil
                case 11:
                    spoilerItem = EntityLocationDict[location].LocationName + ": Shock Coil";
                    break;
                //UAExpansion
                case 18:
                    spoilerItem = EntityLocationDict[location].LocationName + ": UA Expansion";
                    break;
                case 19:
                    spoilerItem = EntityLocationDict[location].LocationName + ": Artifact" + artifactID + artifactModel;
                    break;
            }
            //if (spoiler_chkbox.IsChecked ?? true)
            //{
            using (StreamWriter spoilerLog = new StreamWriter(spoilerLogPath, true))
            {
                spoilerLog.WriteLine(spoilerItem);
            }
            //}
            //these two lines set up the writing
            FileStream ws = new FileStream(fullPath, FileMode.Open, FileAccess.Write);
            StreamWriter writeStream = new StreamWriter(ws);
            //location is the address of where to write to
            writeStream.BaseStream.Seek(EntityLocationDict[location].Offset, SeekOrigin.Begin);
            if (item == 19)
            {
                byte[] notifyEntityID = { (byte)EntityLocationDict[location].NotifyEntityId, 0x00 };
                byte[] collectedMessage = { (byte)EntityLocationDict[location].CollectedMessage };
                byte[] message2Target = { (byte)EntityLocationDict[location].Message2Target, 0x00, 0x00, 0x00 };
                byte[] message2 = { (byte)EntityLocationDict[location].Message2 };
                byte[] message3Target = { (byte)EntityLocationDict[location].Message3Target, 0x00, 0x00, 0x00 };
                byte[] message3 = { (byte)EntityLocationDict[location].Message3 };
                byte[] entityType = { 0x11 };
                writeStream.BaseStream.Write(entityType, 0, entityType.Length);
                writeStream.BaseStream.Seek(EntityLocationDict[location].Offset + 40, SeekOrigin.Begin);
                //those first two bytes are the model id and artifact id
                byte[] bytesToWrite = new byte[32];
                bytesToWrite[0] = artifactID;
                bytesToWrite[1] = artifactModel;
                bytesToWrite[2] = EntityLocationDict[location].Enabled;
                bytesToWrite[3] = EntityLocationDict[location].HasBase;
                bytesToWrite[4] = notifyEntityID[0];
                bytesToWrite[5] = notifyEntityID[1];
                bytesToWrite[6] = 0x00; //notifyEntityID always 0x00
                bytesToWrite[7] = 0x00; //notifyEntityID always 0x00
                bytesToWrite[8] = collectedMessage[0];
                bytesToWrite[9] = 0x00; //collectedMessage
                bytesToWrite[10] = 0x00; //collectedMessage
                bytesToWrite[11] = 0x00; //collectedMessage
                bytesToWrite[12] = message2Target[0];
                bytesToWrite[13] = message2Target[1];
                bytesToWrite[14] = message2Target[2];
                bytesToWrite[15] = message2Target[3];
                bytesToWrite[16] = message2[0];
                bytesToWrite[17] = 0x00; //message 2
                bytesToWrite[18] = 0x00; //message 2
                bytesToWrite[19] = 0x00; //message 2
                bytesToWrite[20] = message3Target[0];
                bytesToWrite[21] = message3Target[1];
                bytesToWrite[22] = message3Target[2];
                bytesToWrite[23] = message3Target[3];
                bytesToWrite[24] = message3[0];
                bytesToWrite[25] = 0x00; //message 3
                bytesToWrite[26] = 0x00; //message 3
                bytesToWrite[27] = 0x00; //message 3
                bytesToWrite[28] = linkedEntity[0]; //linked Entity always -1
                bytesToWrite[29] = linkedEntity[1]; //linked Entity always -1
                bytesToWrite[30] = 0x00; //padding
                bytesToWrite[31] = 0x00; //padding
                writeStream.BaseStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                writeStream.Close();
                if (artifactModel < 2)
                {
                    artifactModel++;
                }
                else
                {
                    artifactModel = 0;
                    artifactID++;
                }
            }
            else
            {
                //write item data here
                byte itemType = (byte)item;
                byte[] notifyEntityID = { (byte)EntityLocationDict[location].NotifyEntityId, 0x00 };
                byte[] collectedMessage = { (byte)EntityLocationDict[location].CollectedMessage };
                byte[] entityType = { 0x04 };
                writeStream.BaseStream.Write(entityType, 0, entityType.Length);
                writeStream.BaseStream.Seek(EntityLocationDict[location].Offset + 40, SeekOrigin.Begin);
                byte[] bytesToWrite = new byte[32];
                bytesToWrite[0] = linkedEntity[0]; //ParentId
                bytesToWrite[1] = linkedEntity[1]; //ParentId
                bytesToWrite[2] = 0x00; //ParentId
                bytesToWrite[3] = 0x00; //ParentId
                bytesToWrite[4] = itemType;
                bytesToWrite[5] = 0x00; //item type
                bytesToWrite[6] = 0x00; //item type
                bytesToWrite[7] = 0x00; //item type
                bytesToWrite[8] = EntityLocationDict[location].Enabled;
                bytesToWrite[9] = EntityLocationDict[location].HasBase;
                bytesToWrite[10] = 0x00; //Always Active
                bytesToWrite[11] = 0x00; //Padding
                bytesToWrite[12] = 0x01; //max spawn count
                bytesToWrite[13] = 0x00; //max spawn count
                bytesToWrite[14] = 0x00; //spawn interval
                bytesToWrite[15] = 0x00; //spawn interval
                bytesToWrite[16] = 0x00; //spawn delay
                bytesToWrite[17] = 0x00; //spawn delay
                bytesToWrite[18] = notifyEntityID[0];
                bytesToWrite[19] = notifyEntityID[1];
                bytesToWrite[20] = collectedMessage[0];
                bytesToWrite[21] = 0x00; //collectedMessage
                bytesToWrite[22] = 0x00; //collectedMessage
                bytesToWrite[23] = 0x00; //collectedMessage
                bytesToWrite[24] = 0x00; //CollectedMsgParam1
                bytesToWrite[25] = 0x00; //CollectedMsgParam1
                bytesToWrite[26] = 0x00; //CollectedMsgParam1
                bytesToWrite[27] = 0x00; //CollectedMsgParam1
                bytesToWrite[28] = 0x00; //CollectedMsgParam2
                bytesToWrite[29] = 0x00; //CollectedMsgParam2
                bytesToWrite[30] = 0x00; //CollectedMsgParam2
                bytesToWrite[31] = 0x00; //CollectedMsgParam2
                writeStream.BaseStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                writeStream.Close();
            }
            if (linkedEntity[0] != 0xFF)
            {
                linkedEntity[0] = 0xFF;
                linkedEntity[1] = 0xFF;
            }
        }

        public void LocationExclude()
        {
            //this makes it so that when one beam is placed it will not place another beam in the locations that require no beam to get to
            //TODO add any artifacts that are obtainable without beams to this list
            bool hasrolled = false;
            int ran = 0;
            int locationExcludedNum = 5;
            while (locationExcludedNum != 0)
            {
                if (hasrolled == false){
                    ran = RandomNumberGenerator(7);
                    hasrolled = true;
                }
                if (starterLocationExcluded[ran] != 1)
                {
                    switch (ran)
                    {
                        case 1:
                            if (locationHasItem[1] != 1)
                            {
                                locationHasItem[1] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 2:
                            if (locationHasItem[2] != 1)
                            {
                                locationHasItem[2] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 3:
                            if (locationHasItem[3] != 1)
                            {
                                locationHasItem[3] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 4:
                            if (locationHasItem[4] != 1)
                            {
                                locationHasItem[4] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 5:
                            if (locationHasItem[5] != 1)
                            {
                                locationHasItem[5] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 6:
                            if (locationHasItem[6] != 1)
                            {
                                locationHasItem[6] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 7:
                            if (locationHasItem[16] != 1)
                            {
                                locationHasItem[16] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 8:
                            if (locationHasItem[17] != 1)
                            {
                                locationHasItem[17] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 9:
                            if (locationHasItem[19] != 1)
                            {
                                locationHasItem[19] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 10:
                            if (locationHasItem[21] != 1)
                            {
                                locationHasItem[21] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 11:
                            if (locationHasItem[22] != 1)
                            {
                                locationHasItem[22] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 12:
                            if (locationHasItem[31] != 1)
                            {
                                locationHasItem[31] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 13:
                            if (locationHasItem[34] != 1)
                            {
                                locationHasItem[34] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 14:
                            if (locationHasItem[42] != 1)
                            {
                                locationHasItem[42] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 15:
                            if (locationHasItem[43] != 1)
                            {
                                locationHasItem[43] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                    }
                }
                if (hasrolled == true)
                {
                    if (ran <= 15)
                    {
                        ran++;
                    }
                    else
                    {
                        ran = 1;
                    }
                }
            }
        }

        public void ExcludePlanets(int beam, int location)
        {
            if (location <= 14)
            {
                planetHasItems[1]++;
            }
            if (location >= 15 && location <= 30)
            {
                planetHasItems[2]++;
            }
            if (location >= 31 && location <= 41)
            {
                planetHasItems[3]++;
            }
            if (location > 42 && location <= 57)
            {
                planetHasItems[4]++;
            }
            beamLocation[beam] = location;

            //alinos
            if (planetHasItems[1] == 2)
            {
                locationExcluded[1] = 1;
                locationExcluded[2] = 1;
                locationExcluded[3] = 1;
                locationExcluded[4] = 1;
                locationExcluded[5] = 1;
                locationExcluded[6] = 1;
                locationExcluded[7] = 1;
                locationExcluded[8] = 1;
                locationExcluded[9] = 1;
                locationExcluded[10] = 1;
                locationExcluded[11] = 1;
                locationExcluded[12] = 1;
                locationExcluded[13] = 1;
                locationExcluded[14] = 1;
                locationHasItem[1] = 1;
                locationHasItem[2] = 1;
                locationHasItem[3] = 1;
                locationHasItem[4] = 1;
                locationHasItem[5] = 1;
                locationHasItem[6] = 1;
                locationHasItem[7] = 1;
                locationHasItem[8] = 1;
                locationHasItem[9] = 1;
                locationHasItem[10] = 1;
                locationHasItem[11] = 1;
                locationHasItem[12] = 1;
                locationHasItem[13] = 1;
                locationHasItem[14] = 1;
            }
            //celestial archives
            if (planetHasItems[2] == 2)
            {
                locationExcluded[15] = 1;
                locationExcluded[16] = 1;
                locationExcluded[17] = 1;
                locationExcluded[18] = 1;
                locationExcluded[19] = 1;
                locationExcluded[20] = 1;
                locationExcluded[21] = 1;
                locationExcluded[22] = 1;
                locationExcluded[23] = 1;
                locationExcluded[24] = 1;
                locationExcluded[25] = 1;
                locationExcluded[26] = 1;
                locationExcluded[27] = 1;
                locationExcluded[28] = 1;
                locationExcluded[29] = 1;
                locationExcluded[30] = 1;
                locationHasItem[15] = 1;
                locationHasItem[16] = 1;
                locationHasItem[17] = 1;
                locationHasItem[18] = 1;
                locationHasItem[19] = 1;
                locationHasItem[20] = 1;
                locationHasItem[21] = 1;
                locationHasItem[22] = 1;
                locationHasItem[23] = 1;
                locationHasItem[24] = 1;
                locationHasItem[25] = 1;
                locationHasItem[26] = 1;
                locationHasItem[27] = 1;
                locationHasItem[28] = 1;
                locationHasItem[29] = 1;
                locationHasItem[30] = 1;
            }
            //vesper defense outpost
            if (planetHasItems[3] == 2)
            {
                locationExcluded[31] = 1;
                locationExcluded[32] = 1;
                locationExcluded[33] = 1;
                locationExcluded[34] = 1;
                locationExcluded[35] = 1;
                locationExcluded[36] = 1;
                locationExcluded[37] = 1;
                locationExcluded[38] = 1;
                locationExcluded[39] = 1;
                locationExcluded[40] = 1;
                locationExcluded[41] = 1;
                locationHasItem[31] = 1;
                locationHasItem[32] = 1;
                locationHasItem[33] = 1;
                locationHasItem[34] = 1;
                locationHasItem[35] = 1;
                locationHasItem[36] = 1;
                locationHasItem[37] = 1;
                locationHasItem[38] = 1;
                locationHasItem[39] = 1;
                locationHasItem[40] = 1;
                locationHasItem[41] = 1;
            }
            //arcterra
            if (planetHasItems[4] == 2)
            {
                locationExcluded[42] = 1;
                locationExcluded[43] = 1;
                locationExcluded[44] = 1;
                locationExcluded[45] = 1;
                locationExcluded[46] = 1;
                locationExcluded[47] = 1;
                locationExcluded[48] = 1;
                locationExcluded[49] = 1;
                locationExcluded[50] = 1;
                locationExcluded[51] = 1;
                locationExcluded[52] = 1;
                locationExcluded[53] = 1;
                locationExcluded[54] = 1;
                locationExcluded[55] = 1;
                locationExcluded[56] = 1;
                locationExcluded[57] = 1;
                locationHasItem[42] = 1;
                locationHasItem[43] = 1;
                locationHasItem[44] = 1;
                locationHasItem[45] = 1;
                locationHasItem[46] = 1;
                locationHasItem[47] = 1;
                locationHasItem[48] = 1;
                locationHasItem[49] = 1;
                locationHasItem[50] = 1;
                locationHasItem[51] = 1;
                locationHasItem[52] = 1;
                locationHasItem[53] = 1;
                locationHasItem[54] = 1;
                locationHasItem[55] = 1;
                locationHasItem[56] = 1;
                locationHasItem[57] = 1;
            }
        }

        public void FixExclusions()
        {
            if (starterLocationExcluded[1] == 1)
            {
                locationHasItem[1] = 0;
            }
            if (starterLocationExcluded[2] == 1)
            {
                locationHasItem[2] = 0;
            }
            if (starterLocationExcluded[3] == 1)
            {
                locationHasItem[3] = 0;
            }
            if (starterLocationExcluded[4] == 1)
            {
                locationHasItem[4] = 0;
            }
            if (starterLocationExcluded[5] == 1)
            {
                locationHasItem[5] = 0;
            }
            if (starterLocationExcluded[6] == 1)
            {
                locationHasItem[6] = 0;
            }
            if (starterLocationExcluded[7] == 1)
            {
                locationHasItem[16] = 0;
            }
            if (starterLocationExcluded[8] == 1)
            {
                locationHasItem[17] = 0;
            }
            if (starterLocationExcluded[9] == 1)
            {
                locationHasItem[19] = 0;
            }
            if (starterLocationExcluded[10] == 1)
            {
                locationHasItem[21] = 0;
            }
            if (starterLocationExcluded[11] == 1)
            {
                locationHasItem[22] = 0;
            }
            if (starterLocationExcluded[12] == 1)
            {
                locationHasItem[31] = 0;
            }
            if (starterLocationExcluded[13] == 1)
            {
                locationHasItem[34] = 0;
            }
            if (starterLocationExcluded[14] == 1)
            {
                locationHasItem[42] = 0;
            }
            if (starterLocationExcluded[15] == 1)
            {
                locationHasItem[43] = 0;
            }
            for (int i = 0; i <= 58; i++)
            {
                locationHasItem[i] = 0;
            }
            locationHasItem[beamLocation[1]] = 1;
            locationHasItem[beamLocation[2]] = 1;
            locationHasItem[beamLocation[3]] = 1;
            locationHasItem[beamLocation[4]] = 1;
            locationHasItem[beamLocation[5]] = 1;
            locationHasItem[beamLocation[6]] = 1;
        }
    }
}