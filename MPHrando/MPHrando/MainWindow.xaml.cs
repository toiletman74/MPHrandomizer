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
        int[] beamToPlace = new int[] {0,0,0,0,0,0,0};
        int[] locationHasItem = new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        int[] locationExcluded = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] beamLocation = new int[] { 0, 0, 0, 0, 0, 0, 0 };
        int[] starterLocationExcluded = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] planetHasItems = new int[] { 0, 0, 0, 0, 0 };
        int randomnum;
        bool hasRolled;
        bool hasRolledBeam;
        bool hasRolledItem;
        bool rollingBeams;
        int seed;
        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void fileSelect_click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = System.Windows.Forms.Application.StartupPath;
            //fbd.SelectedPath = "C:\\Users\\Lenka\\Desktop\\MPH moding tools\\dslazy\\NDS_UNPACK\\data\\levels\\entities";
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
            itemsToPlace = 28;
            missilesToPlace = 9;
            etankToPlace = 7;
            UAEToPlace = 12;
            beamsToPlace = 6;
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
                spoilerLog.WriteLine("MPHrando v0.1.5b");
                spoilerLog.WriteLine(logEntry);
                }
            //}
            //this is the sic transit magmaul barrier patch
            string fullPath = FilePath.pathEntityFldr + "\\unit4_rm3_Ent.bin";
            FileStream ws = new FileStream(fullPath, FileMode.Open, FileAccess.Write);
            StreamWriter writeStream = new StreamWriter(ws);
            writeStream.BaseStream.Seek(20316, SeekOrigin.Current);
            writeStream.BaseStream.Write(new byte[] { 0x00 }, 0, 1);
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
                            ExcludePlanets(Planet,ran,location);
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
                            ExcludePlanets(Planet, ran, location);
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
                            ExcludePlanets(Planet, ran, location);
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
                            ExcludePlanets(Planet, ran, location);
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
                            ExcludePlanets(Planet, ran, location);
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
                            ExcludePlanets(Planet, ran, location);
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
                RollItems();
            }
        }

        public void RollLocation(int item)
        {
            if (hasRolled == false)
            {
                location = RandomNumberGenerator(34);
            }
            else
            {
                location ++;
            }
            switch (location)
            {
                case 1:
                    //VDOCortexCPUBeamLocation
                    if (locationHasItem[1] != 1 && (beamToPlace[2] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit3_C2_Ent.bin";
                        address = 2124;
                        Planet = 3;
                        PlaceItem(item, address, "VDOCortexCPUBeamLocation");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 2:
                    //ARCIceHiveBeamLocation
                    if (locationHasItem[2] != 1 && (beamToPlace[4] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit4_RM1_Ent.bin";
                        address = 10276;
                        Planet = 4;
                        PlaceItem(item, address, "ARCIceHiveBeamLocation");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 3:
                    //CADataShrine02BeamLocation
                    if (locationHasItem[3] != 1 && (beamToPlace[2] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_RM2_Ent.bin";
                        address = 4844;
                        Planet = 1;
                        PlaceItem(item, address, "CADataShrine02BeamLocation");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 4:
                    //CAIncubationVault02
                    if (locationHasItem[4] != 1 && (beamToPlace[1] != 0 && beamToPlace[6] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_RM6_Ent.bin";
                        address = 1168;
                        Planet = 1;
                        PlaceItem(item, address, "CAIncubationVault02");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 5:
                    //ACouncilChamberBeamLocation
                    if (locationHasItem[5] != 1 && (beamToPlace[5] != 0 && beamToPlace[4] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\unit1_rm3_Ent.bin";
                        address = 4192;
                        Planet = 2;
                        PlaceItem(item, address, "ACouncilChamberBeamLocation");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 6:
                    //ARCFaultLine
                    if (locationHasItem[6] != 1 && (beamToPlace[5] != 0 || (beamToPlace[3] != 0 && beamToPlace[4] != 0)))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit4_RM5_Ent.bin";
                        address = 4332;
                        Planet = 4;
                        PlaceItem(item, address, "ARCFaultLine");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 7:
                    //CADataShrine01
                    if (locationHasItem[7] != 1)
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_RM1_Ent.bin";
                        address = 8324;
                        Planet = 1;
                        PlaceItem(item, address, "CADataShrine01");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 8:
                    //CANewArrivalRegistration
                    if (locationHasItem[8] != 1 && (beamToPlace[1] != 0 && beamToPlace[6] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_C7_Ent.bin";
                        address = 11216;
                        Planet = 1;
                        PlaceItem(item, address, "CANewArrivalRegistration");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 9:
                    //AEchoHall
                    if (locationHasItem[9] != 1)
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit1_C0_Ent.bin";
                        address = 3444;
                        Planet = 2;
                        PlaceItem(item, address, "AEchoHall");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 10:
                    //ACouncilChamberEtankLocation
                    if (locationHasItem[10] != 1 && (beamToPlace[5] != 0 && beamToPlace[4] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\unit1_rm3_Ent.bin";
                        address = 4048;
                        Planet = 2;
                        PlaceItem(item, address, "ACouncilChamberEtankLocation");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 11:
                    //ARCSicTransit
                    if (locationHasItem[11] != 1)
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\unit4_rm3_Ent.bin";
                        address = 5112;
                        Planet = 4;
                        PlaceItem(item, address, "ARCSicTransit");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 12:
                    //ARCFrostLabyrinth
                    if (locationHasItem[12] != 1 && (beamToPlace[4] != 0 || (beamToPlace[3] != 0 && beamToPlace[5] != 0)))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit4_C0_Ent.bin";
                        address = 8764;
                        Planet = 4;
                        PlaceItem(item, address, "ARCFrostLabyrinth");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 13:
                    //OGoreaPeek
                    if (locationHasItem[13] != 1 && (beamToPlace[1] != 0 && beamToPlace[2] != 0 && beamToPlace[3] != 0 && beamToPlace[4] != 0 && beamToPlace[5] != 0 && beamToPlace[6] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Gorea_Peek_Ent.bin";
                        address = 312;
                        PlaceItem(item, address, "OGoreaPeek");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 14:
                    //CADataShrine02MissileExpansion
                    if (locationHasItem[14] != 1)
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_RM2_Ent.bin";
                        address = 5212;
                        Planet = 1;
                        PlaceItem(item, address, "CADataShrine02MissileExpansion");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 15:
                    //CADataIncubationVault03 this one expects that you know the bombjump trick
                    if (locationHasItem[15] != 1 && (beamToPlace[1] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_RM7_Ent.bin";
                        address = 988;
                        Planet = 1;
                        PlaceItem(item, address, "CADataIncubationVault03");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 16:
                    //AAlinosGateway
                    if (locationHasItem[16] != 1)
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit1_Land_Ent.bin";
                        address = 2068;
                        Planet = 2;
                        PlaceItem(item, address, "AAlinosGateway");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 17:
                    //AHighGround
                    if (locationHasItem[17] != 1)
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\unit1_RM1_Ent.bin";
                        address = 15916;
                        Planet = 2;
                        PlaceItem(item, address, "AHighGround");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 18:
                    //AAlinosPerch
                    if (locationHasItem[18] != 1 && (beamToPlace[4] != 0 && beamToPlace[5] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\unit1_RM2_Ent.bin";
                        address = 4272;
                        Planet = 2;
                        PlaceItem(item, address, "AAlinosPerch");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 19:
                    //VDOCortexCPUMissileExpansion
                    if (locationHasItem[19] != 1)
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit3_C2_Ent.bin";
                        address = 5872;
                        Planet = 3;
                        PlaceItem(item, address, "VDOCortexCPUMissileExpansion");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 20:
                    //VDOFuelStack
                    if (locationHasItem[20] != 1 && (beamToPlace[2] != 0 && beamToPlace[3] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit3_RM2_Ent.bin";
                        address = 17812;
                        Planet = 3;
                        PlaceItem(item, address, "VDOFuelStack");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 21:
                    //ARCIceHiveMissileExpansion
                    if (locationHasItem[21] != 1 && (beamToPlace[4] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit4_RM1_Ent.bin";
                        address = 24556;
                        Planet = 4;
                        PlaceItem(item, address, "ARCIceHiveMissileExpansion");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 22:
                    //ARCSubterranean
                    if (locationHasItem[22] != 1 && (beamToPlace[3] != 0 && (beamToPlace[4] != 0 || beamToPlace[5] != 0)))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit4_RM2_Ent.bin";
                        address = 7848;
                        Planet = 4;
                        PlaceItem(item, address, "ARCSubterranean");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 23:
                    //CACelestialGateway
                    if (locationHasItem[23] != 1 && (beamToPlace[2] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_Land_Ent.bin";
                        address = 4372;
                        Planet = 1;
                        PlaceItem(item, address, "CACelestialGateway");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 24:
                    //CADataShrine02UAE
                    if (locationHasItem[24] != 1 && (beamToPlace[2] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_RM2_Ent.bin";
                        address = 9668;
                        Planet = 1;
                        PlaceItem(item, address, "CADataShrine02UAE");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 25:
                    //CATransferLock
                    if (locationHasItem[25] != 1 && (beamToPlace[1] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_RM4_Ent.bin";
                        address = 17360;
                        Planet = 1;
                        PlaceItem(item, address, "CATransferLock");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 26:
                    //CADockingBay
                    if (locationHasItem[26] != 1 && beamToPlace[1] != 0)
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit2_RM8_Ent.bin";
                        address = 7324;
                        Planet = 1;
                        PlaceItem(item, address, "CADockingBay");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 27:
                    //AMagmaDrop
                    if (locationHasItem[27] != 1 && (beamToPlace[4] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit1_C4_Ent.bin";
                        address = 3308;
                        Planet = 2;
                        PlaceItem(item, address, "AMagmaDrop");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 28:
                    //AProcessorCore
                    if (locationHasItem[28] != 1 && (beamToPlace[4] != 0 && beamToPlace[5] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\unit1_rm5_Ent.bin";
                        address = 4932;
                        Planet = 2;
                        PlaceItem(item, address, "AProcessorCore");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 29:
                    //VDOCompressionChamber
                    if (locationHasItem[29] != 1 && (beamToPlace[2] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit3_rm4_Ent.bin";
                        address = 6412;
                        Planet = 3;
                        PlaceItem(item, address, "VDOCompressionChamber");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 30:
                    //VDOStaisBunker
                    if (locationHasItem[30] != 1 && (beamToPlace[2] != 0 && beamToPlace[3] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit3_RM3_Ent.bin";
                        address = 19848;
                        Planet = 3;
                        PlaceItem(item, address, "VDOStaisBunker");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 31:
                    //ARCIceHiveUAE *WaspRooom*
                    if (locationHasItem[31] != 1 && (beamToPlace[4] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit4_RM1_Ent.bin";
                        address = 11160;
                        Planet = 4;
                        PlaceItem(item, address, "ARCIceHiveUAE *WaspRooom*");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 32:
                    //ARCIceHiveUAE *cubbie*
                    if (locationHasItem[32] != 1 && (beamToPlace[4] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit4_RM1_Ent.bin";
                        address = 52732;
                        Planet = 4;
                        PlaceItem(item, address, "ARCIceHiveUAE *cubbie*");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 33:
                    //ARCSanctorus
                    if (locationHasItem[33] != 1 && (beamToPlace[3] != 0 && (beamToPlace[4] != 0 || beamToPlace[5] != 0)))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\unit4_rm4_Ent.bin";
                        address = 12220;
                        Planet = 4;
                        PlaceItem(item, address, "ARCSanctorus");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 34:
                    //ARCDripMoat
                    if (locationHasItem[34] != 1 && (beamToPlace[3] != 0 && (beamToPlace[4] != 0 || beamToPlace[5] != 0)))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\unit4_C1_Ent.bin";
                        address = 23840;
                        Planet = 4;
                        PlaceItem(item, address,"ARCDripMoat");
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

        public void PlaceItem(int item, int location, string spoil)
        {
            string spoilerItem = "";
            string fullPath = FilePath.pathEntityFldr + FilePath.fileName;
            string spoilerLogPath = System.Windows.Forms.Application.StartupPath + "\\SpoilerLog.txt";
            switch (item)
            {
                //EnergyTank
                case 4:
                    spoilerItem = spoil + ": Energy Tank";
                    break;
                //VoltDriver
                case 5:
                    spoilerItem = spoil + ": Volt Driver";
                    break;
                //MissileExpansion
                case 6:
                    spoilerItem = spoil + ": Missile Expansion";
                    break;
                //Battlehammer
                case 7:
                    spoilerItem = spoil + ": Battlehammer";
                    break;
                //Imperialist
                case 8:
                    spoilerItem = spoil + ": Imperialist";
                    break;
                //Judicator
                case 9:
                    spoilerItem = spoil + ": Judicator";
                    break;
                //Magmaul
                case 10:
                    spoilerItem = spoil + ": Magmaul";
                    break;
                //ShockCoil
                case 11:
                    spoilerItem = spoil + ": Shock Coil";
                    break;
                //UAExpansion
                case 18:
                    spoilerItem = spoil + ": UA Expansion";
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
            //this line tells the stream writer where to go
            writeStream.BaseStream.Seek(location, SeekOrigin.Current);
            //this variable is the byte that you want to write into the file
            switch (item)
            {
                //EnergyTank
                case 4:
                    valueIn = new byte[] { 0x04 };
                    break;
                //VoltDriver
                case 5:
                    valueIn = new byte[] { 0x05 };
                    break;
                //MissileExpansion
                case 6:
                    valueIn = new byte[] { 0x06 };
                    break;
                //Battlehammer
                case 7:
                    valueIn = new byte[] { 0x07 };
                    break;
                //Imperialist
                case 8:
                    valueIn = new byte[] { 0x08 };
                    break;
                //Judicator
                case 9:
                    valueIn = new byte[] { 0x09 };
                    break;
                //Magmaul
                case 10:
                    valueIn = new byte[] { 0x0A };
                    break;
                //ShockCoil
                case 11:
                    valueIn = new byte[] { 0x0B };
                    break;
                //UAExpansion
                case 18:
                    valueIn = new byte[] { 0x12 };
                    break;
            }
            writeStream.BaseStream.Write(valueIn, 0, 1);
            writeStream.Close();
        }

        public void LocationExclude()
        {
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
                            if (locationHasItem[7] != 1)
                            {
                                locationHasItem[7] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 2:
                            if (locationHasItem[9] != 1)
                            {
                                locationHasItem[9] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 3:
                            if (locationHasItem[11] != 1)
                            {
                                locationHasItem[11] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 4:
                            if (locationHasItem[14] != 1)
                            {
                                locationHasItem[14] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 5:
                            if (locationHasItem[16] != 1)
                            {
                                locationHasItem[16] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 6:
                            if (locationHasItem[17] != 1)
                            {
                                locationHasItem[17] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                        case 7:
                            if (locationHasItem[19] != 1)
                            {
                                locationHasItem[19] = 1;
                                starterLocationExcluded[ran] = 1;
                                locationExcludedNum--;
                                hasrolled = false;
                            }
                            break;
                    }
                }
                if (hasrolled == true)
                {
                    if (ran != 7)
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

        public void ExcludePlanets(int planet, int beam, int location)
        {
            switch (planet) 
            {
                //celestial archives
                case 1:
                    planetHasItems[1] += 1;
                    break;
                //alinos
                case 2:
                    planetHasItems[2] += 1;
                    break;
                //vesper defense outpost
                case 3:
                    planetHasItems[3] += 1;
                    break;
                //arcterra
                case 4:
                    planetHasItems[4] += 1;
                    break;
            }
            beamLocation[beam] = location;
            //celestial archives
            if (planetHasItems[1] == 2)
            {
                locationExcluded[3] = 1;
                locationExcluded[4] = 1;
                locationExcluded[7] = 1;
                locationExcluded[8] = 1;
                locationExcluded[14] = 1;
                locationExcluded[15] = 1;
                locationExcluded[23] = 1;
                locationExcluded[24] = 1;
                locationExcluded[25] = 1;
                locationExcluded[26] = 1;
                locationHasItem[3] = 1;
                locationHasItem[4] = 1;
                locationHasItem[7] = 1;
                locationHasItem[8] = 1;
                locationHasItem[14] = 1;
                locationHasItem[15] = 1;
                locationHasItem[23] = 1;
                locationHasItem[24] = 1;
                locationHasItem[25] = 1;
                locationHasItem[26] = 1;
            }
            //alinos
            if (planetHasItems[2] == 2)
            {
                locationExcluded[5] = 1;
                locationExcluded[9] = 1;
                locationExcluded[10] = 1;
                locationExcluded[16] = 1;
                locationExcluded[17] = 1;
                locationExcluded[18] = 1;
                locationExcluded[27] = 1;
                locationExcluded[28] = 1;
                locationHasItem[5] = 1;
                locationHasItem[9] = 1;
                locationHasItem[10] = 1;
                locationHasItem[16] = 1;
                locationHasItem[17] = 1;
                locationHasItem[18] = 1;
                locationHasItem[27] = 1;
                locationHasItem[28] = 1;
            }
            //vesper defense outpost
            if (planetHasItems[3] == 2)
            {
                locationExcluded[1] = 1;
                locationExcluded[19] = 1;
                locationExcluded[20] = 1;
                locationExcluded[29] = 1;
                locationExcluded[30] = 1;
                locationHasItem[1] = 1;
                locationHasItem[19] = 1;
                locationHasItem[20] = 1;
                locationHasItem[29] = 1;
                locationHasItem[30] = 1;
            }
            //arcterra
            if (planetHasItems[4] == 2)
            {
                locationExcluded[2] = 1;
                locationExcluded[6] = 1;
                locationExcluded[11] = 1;
                locationExcluded[12] = 1;
                locationExcluded[21] = 1;
                locationExcluded[22] = 1;
                locationExcluded[31] = 1;
                locationExcluded[32] = 1;
                locationExcluded[33] = 1;
                locationExcluded[34] = 1;
                locationHasItem[2] = 1;
                locationHasItem[6] = 1;
                locationHasItem[11] = 1;
                locationHasItem[12] = 1;
                locationHasItem[21] = 1;
                locationHasItem[22] = 1;
                locationHasItem[31] = 1;
                locationHasItem[32] = 1;
                locationHasItem[33] = 1;
                locationHasItem[34] = 1;
            }
        }

        public void FixExclusions()
        {
            if (starterLocationExcluded[1] == 1)
            {
                locationHasItem[7] = 0;
            }
            if (starterLocationExcluded[2] == 1)
            {
                locationHasItem[9] = 0;
            }
            if (starterLocationExcluded[3] == 1)
            {
                locationHasItem[11] = 0;
            }
            if (starterLocationExcluded[4] == 1)
            {
                locationHasItem[14] = 0;
            }
            if (starterLocationExcluded[5] == 1)
            {
                locationHasItem[16] = 0;
            }
            if (starterLocationExcluded[6] == 1)
            {
                locationHasItem[17] = 0;
            }
            if (starterLocationExcluded[7] == 1)
            {
                locationHasItem[19] = 0;
            }
            for (int i = 0; i <= 34; i++)
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