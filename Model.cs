using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKUassignment
{
    class Model
    {
        /// <summary>
        /// Marketing name. For example: GALAXY A12
        /// </summary>
        public string MktName { get; set; }
        /// <summary>
        /// Model number. For example: SM-A025GZKEEUE
        /// </summary>
        public string ModelNumber { get; set; }
        public List<Packcode> Packcodes { get; set; }
        public bool Found { get; set; }
        public Model(string model, string PkgCode)
        {
            this.MktName = model.Trim();
            this.ModelNumber = model.Trim();
            this.Packcodes = new List<Packcode>();
            this.Packcodes.Add(new Packcode(PkgCode.Trim()));
            this.Found = false;
            /*
            //Defined Key pairs for codes
            List<KeyValuePair<string, string>> DevicesPackcodesPairs = new List<KeyValuePair<string, string>>();
            //Fold
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Fold,Galaxy Z Fold2 5G,Galaxy Z Fold3 5G", "P-GT-OXXCS0FV,P-GT-NXXCS0FV,P-GT-NXXLS0FV,P-GT-OXXCS0FH,P-GT-NXXCS0FH,P-GT-NXXLS0FH"));
            //Flip
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Z Flip,Galaxy Z Flip 5G,Galaxy Z Flip 3 5G", "P-GT-OXXCS0GV,P-GT-NXXCS0GV,P-GT-NXXLS0GV,P-GT-OXXCS0GH,P-GT-NXXCS0GH,P-GT-NXXLS0GH"));
            //Flagship
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Note 10+ / Galaxy Note 10+ 5G / Galaxy S10 Lite / Galaxy Note 20 / Galaxy S10e / Galaxy Note 20 5G / Galaxy S10 / Galaxy Note 20 Ultra 5G / Galaxy S10+ / Galaxy S20 / Galaxy S20 5G / Galaxy S20 Ultra / Galaxy S20+ / Galaxy S20+ 5G / Galaxy S21 5G / Galaxy S21+ 5G / Galaxy S21 Ultra 5G / Galaxy Note 10".Replace(" / ", ","), "P-GT-OXXCS0PV,P-GT-NXXCS0PV,P-GT-NXXLS0PV,P-GT-OXXCS0PH,P-GT-NXXCS0PH,P-GT-NXXLS0PH"));
            //A High
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy A71 / Galaxy A72 / GALAXY Xcover Pro / Galaxy S20 FE (Qualcomm) / Galaxy S20 FE (Exynos) / Galaxy S20 FE 5G / Galaxy Note 10 Lite / Galaxy S10 Lite".Replace(" / ",","), "P-GT-OXXCS0HV,P-GT-NXXCS0HV,P-GT-NXXLS0HV,P-GT-OXXCS0HH,P-GT-NXXCS0HH,P-GT-NXXLS0HH"));
            //A Mid
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy M51 / Galaxy A10 / Galaxy A31 / Galaxy A32 / Galaxy M52 5G / Galaxy Xcover5 / Galaxy A32 5G / Galaxy A40 / Galaxy A41 / Galaxy A42 5G / Galaxy A51 / Galaxy A52 / Galaxy A52 5G / Galaxy A52s 5G / Galaxy M31s".Replace(" / ", ","), "P-GT-OXXCS0MV,P-GT-NXXCS0MV,P-GT-NXXLS0MV,P-GT-OXXCS0MH,P-GT-NXXCS0MH,P-GT-NXXLS0MH"));
            //A Low
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy A03 / Galaxy M12 / Galaxy M22 / Galaxy A02s / Galaxy A03s / Galaxy A12 / Galaxy A20e / Galaxy A20s / Galaxy A21s / Galaxy A22 / Galaxy A22 5G / Galaxy M11 / Galaxy M21".Replace(" / ", ","), "P-GT-OXXCS0LV,P-GT-NXXCS0LV,P-GT-NXXLS0LV,P-GT-OXXCS0LH,P-GT-NXXCS0LH,P-GT-NXXLS0LH"));
            //Tablet High
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Tab Active Pro / Galaxy Tab Active Pro LTE / Galaxy Tab S7 / Galaxy Tab S7 11.0 / Galaxy Tab S7 12.4 / Galaxy Tab S7 LTE / Galaxy Tab S7+ / Galaxy Tab S7+ 5G / Galaxy Tab S7 FE WiFi / Galaxy Tab S7 FE 5G".Replace(" / ", ","), "P-GT-OXXCT0HV,P-GT-NXXCT0HV,P-GT-NXXLT0HV,P-GT-OXXCT0HH,P-GT-NXXCT0HH,P-GT-NXXLT0HH"));
            //Tablet Low
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Tab S6 / Galaxy Tab S6 LTE / Galaxy Tab A8.0 LTE / Galaxy Tab A 8.0 / Galaxy Tab A7 / Galaxy Tab A7 LTE / Galaxy Tab A7 Lite Wifi / Galaxy Tab A10.1(WiFi) / Galaxy Tab A10.1 / Galaxy Tab S5e / Galaxy Tab S5e LTE / Galaxy Tab S6 Lite(WiFi) / Galaxy Tab S6 Lite / Galaxy Tab Active2 / Galaxy Tab Active2 LTE / Galaxy Tab Active 3 / Galaxy Tab Active 3(Enterprise Edition) / Galaxy Tab A7 Lite / Galaxy Tab A7 Lite LTE".Replace(" / ", ","), "P-GT-OXXCT0LV,P-GT-NXXCT0LV,P-GT-NXXLT0LV,P-GT-OXXCT0LH,P-GT-NXXCT0LH,P-GT-NXXLT0LH"));
            //Wearables
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Buds+ / Galaxy Buds2 / Galaxy Fit2 / Galaxy Watch / Galaxy Watch3 / Galaxy Watch4 / Galaxy Watch4 Classic 46mm BT / Galaxy Watch4 Classic 46mm LTE / Galaxy Watch4 44mm LTE / Galaxy Watch4 44mm BT / Galaxy Watch4 40mm LTE / Galaxy Watch4 40mm BT / Galaxy Watch4 Classic 42mm BT / Galaxy Watch4 Classic 42mm LTE / Galaxy Fit 2 / Galaxy Fit e / Galaxy Watch Active / Galaxy Watch Active2 / Galaxy Watch Active2 (40mm) / Galaxy Watch 3 / Galaxy Watch 3 (41mm) / Galaxy Watch 4 / Galaxy Watch 4 (44mm) / Galaxy Watch 4 Classic / Galaxy Buds + / Galaxy Buds 2 / Galaxy Buds Live / Galaxy Buds Pro".Replace(" / ", ","), "P-GT-OXXCW0MV,P-GT-NXXCW0MV,P-GT-NXXLW0MV,P-GT-OXXCW0MH,P-GT-NXXCW0MH,P-GT-NXXLW0MH"));

            //Searches for Packcode in pairs
            bool found = false;
            for(int i = 0;i<= DevicesPackcodesPairs.Count - 1; i++)
            {
                if (DevicesPackcodesPairs.ElementAt(i).Key.ToUpper().Contains(MktName.ToUpper()))
                {
                    //found the correct codes
                    found = true;
                    string[] codes = DevicesPackcodesPairs.ElementAt(i).Value.Split(',');
                    for(int j = 0; j <= codes.Length - 1; j++)
                    {
                        this.Packcodes.Add(new Packcode(codes[j]));
                    }
                }
            }
            if(found == false)
            {
                Program.Error($"{this.MktName} / {this.ModelNumber} Packcode not found!");
            }*/
        }
        /// <summary>
        /// Checks whether the given packnumber is present in current dataset. Helps with searching
        /// </summary>
        /// <param name="PackNumber">e.g. P-GT-OXXCS0LV</param>
        /// <returns></returns>
        public static bool IsPackcodePresent(string PackNumber)
        {
            //Defined Key pairs for codes
            List<KeyValuePair<string, string>> DevicesPackcodesPairs = new List<KeyValuePair<string, string>>();
            //Fold
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Fold,Galaxy Z Fold2 5G,Galaxy Z Fold3 5G", "P-GT-OXXCS0FV,P-GT-NXXCS0FV,P-GT-NXXLS0FV,P-GT-OXXCS0FH,P-GT-NXXCS0FH,P-GT-NXXLS0FH"));
            //Flip
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Z Flip,Galaxy Z Flip 5G,Galaxy Z Flip 3 5G", "P-GT-OXXCS0GV,P-GT-NXXCS0GV,P-GT-NXXLS0GV,P-GT-OXXCS0GH,P-GT-NXXCS0GH,P-GT-NXXLS0GH"));
            //Flagship
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Note 10+ / Galaxy Note 10+ 5G / Galaxy S10 Lite / Galaxy Note 20 / Galaxy S10e / Galaxy Note 20 5G / Galaxy S10 / Galaxy Note 20 Ultra 5G / Galaxy S10+ / Galaxy S20 / Galaxy S20 5G / Galaxy S20 Ultra / Galaxy S20+ / Galaxy S20+ 5G / Galaxy S21 5G / Galaxy S21+ 5G / Galaxy S21 Ultra 5G / Galaxy Note 10".Replace(" / ", ","), "P-GT-OXXCS0PV,P-GT-NXXCS0PV,P-GT-NXXLS0PV,P-GT-OXXCS0PH,P-GT-NXXCS0PH,P-GT-NXXLS0PH"));
            //A High
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy A71 / Galaxy A72 / Galaxy S20 FE (Qualcomm) / Galaxy S20 FE (Exynos) / Galaxy S20 FE 5G / Galaxy Note 10 Lite / Galaxy S10 Lite".Replace(" / ", ","), "P-GT-OXXCS0HV,P-GT-NXXCS0HV,P-GT-NXXLS0HV,P-GT-OXXCS0HH,P-GT-NXXCS0HH,P-GT-NXXLS0HH"));
            //A Mid
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy M51 / Galaxy A10 / Galaxy A31 / Galaxy A32 / Galaxy A32 5G / Galaxy A40 / Galaxy A41 / Galaxy A42 5G / Galaxy A51 / Galaxy A52 / Galaxy A52 5G / Galaxy A52s 5G / Galaxy M31s".Replace(" / ", ","), "P-GT-OXXCS0MV,P-GT-NXXCS0MV,P-GT-NXXLS0MV,P-GT-OXXCS0MH,P-GT-NXXCS0MH,P-GT-NXXLS0MH"));
            //A Low
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy A03 / Galaxy M12 / Galaxy M22 / Galaxy A02s / Galaxy A03s / Galaxy A12 / Galaxy A20e / Galaxy A20s / Galaxy A21s / Galaxy A22 / Galaxy A22 5G / Galaxy M11 / Galaxy M21".Replace(" / ", ","), "P-GT-OXXCS0LV,P-GT-NXXCS0LV,P-GT-NXXLS0LV,P-GT-OXXCS0LH,P-GT-NXXCS0LH,P-GT-NXXLS0LH"));
            //Tablet High
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Tab Active Pro / Galaxy Tab Active Pro LTE / Galaxy Tab S7 / Galaxy Tab S7 11.0 / Galaxy Tab S7 12.4 / Galaxy Tab S7 LTE / Galaxy Tab S7+ / Galaxy Tab S7+ 5G / Galaxy Tab S7 FE WiFi / Galaxy Tab S7 FE 5G".Replace(" / ", ","), "P-GT-OXXCT0HV,P-GT-NXXCT0HV,P-GT-NXXLT0HV,P-GT-OXXCT0HH,P-GT-NXXCT0HH,P-GT-NXXLT0HH"));
            //Tablet Low
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Tab S6 / Galaxy Tab S6 LTE / Galaxy Tab A8.0 LTE / Galaxy Tab A 8.0 / Galaxy Tab A7 / Galaxy Tab A7 LTE / Galaxy Tab A7 Lite Wifi / Galaxy Tab A10.1(WiFi) / Galaxy Tab A10.1 / Galaxy Tab S5e / Galaxy Tab S5e LTE / Galaxy Tab S6 Lite(WiFi) / Galaxy Tab S6 Lite / Galaxy Tab Active2 / Galaxy Tab Active2 LTE / Galaxy Tab Active 3 / Galaxy Tab Active 3(Enterprise Edition) / Galaxy Tab A7 Lite / Galaxy Tab A7 Lite LTE".Replace(" / ", ","), "P-GT-OXXCT0LV,P-GT-NXXCT0LV,P-GT-NXXLT0LV,P-GT-OXXCT0LH,P-GT-NXXCT0LH,P-GT-NXXLT0LH"));
            //Wearables
            DevicesPackcodesPairs.Add(new KeyValuePair<string, string>("Galaxy Buds+ / Galaxy Buds2 / Galaxy Fit2 / Galaxy Watch / Galaxy Watch3 / Galaxy Watch4 / Galaxy Watch4 Classic 46mm BT / Galaxy Watch4 Classic 46mm LTE / Galaxy Watch4 44mm LTE / Galaxy Watch4 44mm BT / Galaxy Watch4 40mm LTE / Galaxy Watch4 40mm BT / Galaxy Watch4 Classic 42mm BT / Galaxy Watch4 Classic 42mm LTE / Galaxy Fit 2 / Galaxy Fit e / Galaxy Watch Active / Galaxy Watch Active2 / Galaxy Watch Active2 (40mm) / Galaxy Watch 3 / Galaxy Watch 3 (41mm) / Galaxy Watch 4 / Galaxy Watch 4 (44mm) / Galaxy Watch 4 Classic / Galaxy Buds + / Galaxy Buds 2 / Galaxy Buds Live / Galaxy Buds Pro".Replace(" / ", ","), "P-GT-OXXCW0MV,P-GT-NXXCW0MV,P-GT-NXXLW0MV,P-GT-OXXCW0MH,P-GT-NXXCW0MH,P-GT-NXXLW0MH"));
            bool IsFound = false;
            for (int i = 0; i <= DevicesPackcodesPairs.Count - 1; i++)
            {
                string[] codes = DevicesPackcodesPairs.ElementAt(i).Value.Split(',');
                for(int j = 0;j<= codes.Length - 1; j++)
                {
                    if (codes[j] == PackNumber)
                    {
                        IsFound = true;
                        break;
                    }
                }
            }
            return IsFound;
        }
        public override string ToString()
        {
            return ModelNumber.ToString();
        }
    }
    class Packcode
    {
        public string ProductGroup { get; set; }
        public string ProductCategory { get; set; }
        public string Code { get; set; }
        public Packcode(string _code)
        {
            this.Code = _code;
            this.ProductGroup = "GT";
            this.ProductCategory = _code.Substring(_code.Length-4,3);
        }
        public override string ToString()
        {
            return Code.ToString();
        }
    }
}
