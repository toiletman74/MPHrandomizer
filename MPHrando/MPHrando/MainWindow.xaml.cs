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
        int itemsToPlace;
        int missilesToPlace;
        int etankToPlace;
        int UAEToPlace;
        int beamsToPlace;
        int[] beamToPlace = new int[] {0,0,0,0,0,0,0};
        int[] locationHasItem = new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        int[] locationWasExcluded = new int[] {0,0,0,0};
        int[] locationExcluded = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
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
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FilePath.pathEntityFldr = fbd.SelectedPath;
            }
        }

        private void Randomize_click(object sender, RoutedEventArgs e)
        {
            if (seed_txtbox.Text != "")
            {
            seed = Int32.Parse(seed_txtbox.Text);
            Random rnd = new Random(seed);
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
            for (int i = 1; i < beamsToPlace; i++)
            {
                beamToPlace[i] = 0;
            }
            for (int i = 1; i < itemsToPlace + beamsToPlace; i++)
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
                        location = 2124;
                        PlaceItem(item, location, "VDOCortexCPUBeamLocation");
                    }
                    else
                    {
                        hasRolled = true;
                        RollLocation(item);
                    }
                    break;
                case 2:
                    //ArcIceHiveBeamLocation
                    if (locationHasItem[2] != 1 && (beamToPlace[4] != 0))
                    {
                        locationHasItem[location] = 1;
                        FilePath.fileName = "\\Unit4_RM1_Ent.bin";
                        location = 10276;
                        PlaceItem(item, location, "ArcIceHiveBeamLocation");
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
                        location = 4844;
                        PlaceItem(item, location, "CADataShrine02BeamLocation");
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
                        location = 1168;
                        PlaceItem(item, location, "CAIncubationVault02");
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
                        location = 4192;
                        PlaceItem(item, location, "ACouncilChamberBeamLocation");
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
                        location = 4332;
                        PlaceItem(item, location, "ARCFaultLine");
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
                        location = 8324;
                        PlaceItem(item, location, "CADataShrine01");
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
                        location = 11216;
                        PlaceItem(item, location, "CANewArrivalRegistration");
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
                        location = 3444;
                        PlaceItem(item, location, "AEchoHall");
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
                        location = 4048;
                        PlaceItem(item, location, "ACouncilChamberEtankLocation");
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
                        location = 5112;
                        PlaceItem(item, location, "ARCSicTransit");
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
                        location = 8764;
                        PlaceItem(item, location, "ARCFrostLabyrinth");
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
                        location = 312;
                        PlaceItem(item, location, "OGoreaPeek");
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
                        location = 5212;
                        PlaceItem(item, location, "CADataShrine02MissileExpansion");
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
                        location = 988;
                        PlaceItem(item, location, "CADataIncubationVault03");
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
                        location = 2068;
                        PlaceItem(item, location, "AAlinosGateway");
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
                        location = 15916;
                        PlaceItem(item, location, "AHighGround");
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
                        location = 4272;
                        PlaceItem(item, location, "AAlinosPerch");
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
                        location = 5872;
                        PlaceItem(item, location, "VDOCortexCPUMissileExpansion");
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
                        location = 17812;
                        PlaceItem(item, location, "VDOFuelStack");
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
                        location = 24556;
                        PlaceItem(item, location, "ARCIceHiveMissileExpansion");
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
                        location = 7848;
                        PlaceItem(item, location, "ARCSubterranean");
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
                        location = 4372;
                        PlaceItem(item, location, "CACelestialGateway");
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
                        location = 9668;
                        PlaceItem(item, location, "CADataShrine02UAE");
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
                        location = 17360;
                        PlaceItem(item, location, "CATransferLock");
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
                        location = 7324;
                        PlaceItem(item, location, "CADockingBay");
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
                        location = 3308;
                        PlaceItem(item, location, "AMagmaDrop");
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
                        location = 4932;
                        PlaceItem(item, location, "AProcessorCore");
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
                        location = 6412;
                        PlaceItem(item, location, "VDOCompressionChamber");
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
                        location = 19848;
                        PlaceItem(item, location, "VDOStaisBunker");
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
                        location = 11160;
                        PlaceItem(item, location, "ARCIceHiveUAE *WaspRooom*");
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
                        location = 52732;
                        PlaceItem(item, location, "ARCIceHiveUAE *cubbie*");
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
                        location = 12220;
                        PlaceItem(item, location, "ARCSanctorus");
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
                        location = 23840;
                        PlaceItem(item, location,"ARCDripMoat");
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
            int ran = rnd.Next(1, maxnum);
            seed = ran;
            return ran;
        }

        public void PlaceItem(int item, int location, string spoil)
        {
            string fullPath = FilePath.pathEntityFldr + FilePath.fileName;
            string spoilerLogPath = System.Windows.Forms.Application.StartupPath + "\\SpoilerLog.txt";
            string logEntry = spoil + ": " + item.ToString();
            //if (spoiler_chkbox.IsChecked ?? true)
            //{
                using (StreamWriter spoilerLog = new StreamWriter(spoilerLogPath, true))
                {
                    spoilerLog.WriteLine(logEntry);
                }
            //}
            textblock.Text = "Wrote to: " + fullPath;
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
                }
                hasrolled = false;
                if (locationHasItem[ran] != 1)
                {
                    if (locationExcluded[ran] != 1)
                    {
                        switch (ran)
                        {
                        case 1:
                            locationHasItem[7] = 1;
                            locationExcluded[ran] = 1;
                            locationExcludedNum--;
                            break;
                        case 2:
                            locationHasItem[9] = 1;
                            locationExcluded[ran] = 1;
                            locationExcludedNum--;
                            break;
                        case 3:
                            locationHasItem[11] = 1;
                            locationExcluded[ran] = 1;
                            locationExcludedNum--;
                            break;
                        case 4:
                            locationHasItem[14] = 1;
                            locationExcluded[ran] = 1;
                            locationExcludedNum--;
                            break;
                        case 5:
                            locationHasItem[16] = 1;
                            locationExcluded[ran] = 1;
                            locationExcludedNum--;
                            break;
                        case 6:
                            locationHasItem[17] = 1;
                            locationExcluded[ran] = 1;
                            locationExcludedNum--;
                            break;
                        case 7:
                            locationHasItem[19] = 1;
                            locationExcluded[ran] = 1;
                            locationExcludedNum--;
                            break;
                        }
                    }
                    else if (ran != 7)
                    {
                        ran++;
                        hasrolled = true;
                    }
                    else
                    {
                        ran = 1;
                        hasrolled = true;
                    }
                }
                else if (ran != 7)
                { 
                    ran++;
                    hasrolled = true;
                }
                else 
                {
                    ran = 1;
                    hasrolled = true;
                }
            }
        }

        public void FixExclusions()
        {
            if (locationExcluded[1] == 1)
            {
                locationHasItem[7] = 0;
            }
            if (locationExcluded[2] == 1)
            {
                locationHasItem[9] = 0;
            }
            if (locationExcluded[3] == 1)
            {
                locationHasItem[11] = 0;
            }
            if (locationExcluded[4] == 1)
            {
                locationHasItem[14] = 0;
            }
            if (locationExcluded[5] == 1)
            {
                locationHasItem[16] = 0;
            }
            if (locationExcluded[6] == 1)
            {
                locationHasItem[17] = 0;
            }
            if (locationExcluded[7] == 1)
            {
                locationHasItem[19] = 0;
            }
        }
    }
}