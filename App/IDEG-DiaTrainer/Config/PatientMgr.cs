using IDEG_DiaTrainer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Config
{
    public class PatientMgr
    {
        private static Dictionary<Enums.ModelType, List<PatientRecord>> DefaultCohort = new Dictionary<Enums.ModelType, List<PatientRecord>>{
            {
                Enums.ModelType.GCTv2, new List<PatientRecord>
                {
                    new PatientRecord{ Id = 1, Age = 35, Name = "Patient 1 (GCT)", Diabetes = Enums.DiabetesType.Type1, ParameterString = "0 0 0 0 0 0 0 3 1 3 50 8 4 0.5 1.4 1 5 0.1 0.144 0.144 0.001 0.1 1e-05 0.0001 1e-05 0 100 100 100 100 50 50 1 0.2 0.02 0.5 0.006944444444444445 0.003472222222222222 0.0006944444444444445 100 100 100 0 0 0 0 8 8 9 380 8 5 0.8 8 5 45 0.7 0.38519 0.38519 0.8 0.3 0.01 0.1 0.001 0 200 200 342 200 180 100 20 0.5 0.03 0.98 0.03055555555555556 0.04166666666666667 0.003472222222222222 500 500 500 500 500 500 200 10 10 20 1000 14 8 1.5 24 50 120 1 14.4 14.4 0.8 2 0.01 0.1 0.1 0.05 500 500 500 500 250 250 50 10 0.1 0.98 0.08333333333333333 0.2083333333333333 0.08333333333333333" },
                }
            },
            {
                Enums.ModelType.Bases, new List<PatientRecord>
                {
                    new PatientRecord{ Id = 1, Age = 35, Name = "Patient 1 (Bases)", Diabetes = Enums.DiabetesType.Type1, ParameterString = "0.05 0.05 0.25 -3 0.0001 0.001 0.001 -0.01041666666666667 -0.01041666666666667 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 0 3 -1 0 0 0 0.75 35 30 0.1 0.12 0.4 0.05000003348342125 0.3719896097390821 0.6354153897565117 2.999993222357193 0.006564797248322683 1.996356811430162 0.4999973999120048 -0.006545266105070348 0.04166666666666666 -0.2416477418341191 0.8999919619550478 0.2999140895910565 -0.7677592386117823 0.896819089935838 0.07740265634494041 1.734396789412239 0.1104689246382145 0.1998692676931243 0.9069822316606672 0.7708055640967928 0.0791070771355412 -0.2069864150421918 0.8999972674317342 0.2990692763364851 -0.9799724525162214 0.2690292461606456 0.1587600201027732 0.08266086950008318 0.3527266430188042 0.05001000967689229 1.221866855723666 0.1779289177514273 0.1911375812309319 1.999992743460905 0.8999892094201754 0.09112565913069011 -1.999986378677759 0.1000745306275651 0.2793590381243105 1.999999012143357 0.5050055725718575 0.08239173388474137 0.7904613503236841 4.346605219939125 0.9999629489229229 0.8224935209389294 49.97901016448262 4.175492375035708e-07 0.750186693676564 53.31300270262199 59.99985922484883 0.132158555668492 0.1211873026960382 0.8499999996781322 0.75 0.7 1 3 1 2 0.5 0.04166666666666666 0.04166666666666666 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 15 25 1 10 50 10 0.9 70 60 0.145 0.15 0.85" },
                    new PatientRecord{ Id = 2, Age = 40, Name = "Patient 2 (Bases)", Diabetes = Enums.DiabetesType.Type1, ParameterString = "0.05 0.05 0.25 -3 0.0001 0.001 0.001 -0.01041666666666667 -0.01041666666666667 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 0 3 -1 0 0 0 0.75 35 30 0.1 0.12 0.4 0.4176744007865604 0.2427935354911005 0.2503207476749269 2.691941613876648 0.00490292543158153 0.6450777750157477 0.3403187108007648 0.03510166430281526 0.03276621528824068 -1.468794468488297 0.6134984091557489 0.2224288404800008 0.06401719395534687 0.6681256615823736 0.2977309829168978 1.958643948413906 0.8973360805531662 0.2992323476188273 1.99984882919045 0.4993018878035223 0.2997733551259723 -0.9117880550247381 0.3624580135406549 0.2146070570394233 -0.9034676236062386 0.5251987430663806 0.2996363349730209 -1.981813002408461 0.8794448791304696 0.1535895713096915 -0.391641526392523 0.1 0.2724257680536702 -1.97349466486692 0.7533636723969342 0.1536155889441082 -1.790974528988117 0.753180461196896 0.2937763450469056 -1.62897682393404 0.8223582177074003 0.05328547606788492 0 7.037918844287945 0.8728848287820067 5.222204786140026 13.31187822327747 4.56621091814944 0.8854141422886457 65 49.99987037554641 0.145 0.149848647331354 0.7906194659785309 0.75 0.7 1 3 1 2 0.5 0.04166666666666666 0.04166666666666666 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 -0.2 0.9 0.3 -0.2 0.9 0.3 -0.2 0.9 0.3 -0.2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 15 25 1 10 50 10 0.9 65 50 0.145 0.15 0.85"},
                }
            },
        };

        private static Dictionary<Enums.ModelType, string> DefaultParameters = new Dictionary<Enums.ModelType, string> {
            {
                Enums.ModelType.GCTv2, "0 0 0 0 0 0 0 3 1 3 50 8 4 0.5 1.4 1 5 0.1 0.144 0.144 0.001 0.1 1e-05 0.0001 1e-05 0 100 100 100 100 50 50 1 0.2 0.02 0.5 0.006944444444444445 0.003472222222222222 0.0006944444444444445 100 100 100 0 0 0 0 8 8 9 380 8 5 0.8 8 5 45 0.7 0.38519 0.38519 0.8 0.3 0.01 0.1 0.001 0 200 200 342 200 180 100 20 0.5 0.03 0.98 0.03055555555555556 0.04166666666666667 0.003472222222222222 500 500 500 500 500 500 200 10 10 20 1000 14 8 1.5 24 50 120 1 14.4 14.4 0.8 2 0.01 0.1 0.1 0.05 500 500 500 500 250 250 50 10 0.1 0.98 0.08333333333333333 0.2083333333333333 0.08333333333333333"
            },
            {
                Enums.ModelType.Bases, "0.05 0.05 0.25 -3 0.0001 0.001 0.001 -0.01041666666666667 -0.01041666666666667 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 0 3 -1 0 0 0 0.75 35 30 0.1 0.12 0.4 0.4176744007865604 0.2427935354911005 0.2503207476749269 2.691941613876648 0.00490292543158153 0.6450777750157477 0.3403187108007648 0.03510166430281526 0.03276621528824068 -1.468794468488297 0.6134984091557489 0.2224288404800008 0.06401719395534687 0.6681256615823736 0.2977309829168978 1.958643948413906 0.8973360805531662 0.2992323476188273 1.99984882919045 0.4993018878035223 0.2997733551259723 -0.9117880550247381 0.3624580135406549 0.2146070570394233 -0.9034676236062386 0.5251987430663806 0.2996363349730209 -1.981813002408461 0.8794448791304696 0.1535895713096915 -0.391641526392523 0.1 0.2724257680536702 -1.97349466486692 0.7533636723969342 0.1536155889441082 -1.790974528988117 0.753180461196896 0.2937763450469056 -1.62897682393404 0.8223582177074003 0.05328547606788492 0 7.037918844287945 0.8728848287820067 5.222204786140026 13.31187822327747 4.56621091814944 0.8854141422886457 65 49.99987037554641 0.145 0.149848647331354 0.7906194659785309 0.75 0.7 1 3 1 2 0.5 0.04166666666666666 0.04166666666666666 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 -0.2 0.9 0.3 -0.2 0.9 0.3 -0.2 0.9 0.3 -0.2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 15 25 1 10 50 10 0.9 65 50 0.145 0.15 0.85"
            },
        };

        private static Dictionary<Enums.ModelType, Dictionary<string, Tuple<int, string>>> CustomCohort = new Dictionary<Enums.ModelType, Dictionary<string, Tuple<int, string>>>
        {
        };

        private static Dictionary<int, string> CustomDataFiles = new Dictionary<int, string>
        {
        };

        private static int MaxCustomPatientId = 0;

        private static void LoadCustomCohort()
        {
            CustomCohort.Clear();

            // GCTv2
            {
                CustomCohort.Add(Enums.ModelType.GCTv2, new Dictionary<string, Tuple<int, string>>());
                var path = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "custom_cohort_gct2.csv");

                if (File.Exists(path))
                {
                    var lines = File.ReadAllLines(path);

                    foreach (var line in lines)
                    {
                        var parts = line.Split(';');
                        if (parts.Length != 3 && (parts[0].Length == 0 || parts[1].Length == 0 || parts[2].Length == 0))
                            continue;

                        var name = parts[0];
                        int id = int.Parse(parts[1]);
                        var parameters = parts[2];

                        CustomCohort[Enums.ModelType.GCTv2].Add(name, new Tuple<int, string>(id, parameters));

                        if (id > MaxCustomPatientId)
                            MaxCustomPatientId = id;
                    }
                }
            }

            // Bases
            {
                CustomCohort.Add(Enums.ModelType.Bases, new Dictionary<string, Tuple<int, string>>());
                var path = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "custom_cohort_bases.csv");
                if (File.Exists(path))
                {
                    var lines = File.ReadAllLines(path);

                    foreach (var line in lines)
                    {
                        var parts = line.Split(';');
                        if (parts.Length != 3 && (parts[0].Length == 0 || parts[1].Length == 0 || parts[2].Length == 0))
                            continue;

                        var name = parts[0];
                        int id = int.Parse(parts[1]);
                        var parameters = parts[2];

                        CustomCohort[Enums.ModelType.Bases].Add(name, new Tuple<int, string>(id, parameters));

                        if (id > MaxCustomPatientId)
                            MaxCustomPatientId = id;
                    }
                }
            }


            // common
            {
                CustomDataFiles.Clear();
                var path = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "custom_data_files.csv");
                if (File.Exists(path))
                {
                    var lines = File.ReadAllLines(path);

                    foreach (var line in lines)
                    {
                        var parts = line.Split(';');
                        if (parts.Length != 2 && (parts[0].Length == 0 || parts[1].Length == 0))
                            continue;

                        int id = int.Parse(parts[0]);
                        string filename = parts[1];

                        CustomDataFiles.Add(id, filename);
                    }
                }
            }
        }

        public static string GetCustomDataFileFor(int patientId)
        {
            if (CustomCohort.Count == 0)
                LoadCustomCohort();

            if (CustomDataFiles.ContainsKey(patientId))
                return CustomDataFiles[patientId];

            return null;
        }

        public static void StoreCustomDataFileFor(int patientId, string tempFilePath)
        {
            if (!Directory.Exists(Microsoft.Maui.Storage.FileSystem.AppDataDirectory))
                Directory.CreateDirectory(Microsoft.Maui.Storage.FileSystem.AppDataDirectory);

            string basePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "datafiles");
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            basePath = Path.Combine(basePath, patientId.ToString() + ".data");

            File.Copy(tempFilePath, basePath);

            CustomDataFiles.Add(patientId, basePath);

            SaveCustomCohort();
        }

        private static async void SaveCustomCohort()
        {
            if (CustomCohort.Count == 0)
                LoadCustomCohort();

            if (!Directory.Exists(Microsoft.Maui.Storage.FileSystem.AppDataDirectory))
                Directory.CreateDirectory(Microsoft.Maui.Storage.FileSystem.AppDataDirectory);

            // GCTv2
            using (StreamWriter file = new(Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "custom_cohort_gct2.csv")))
            {
                foreach (var pat in CustomCohort[Enums.ModelType.GCTv2])
                {
                    string line = pat.Key + ";" + pat.Value.Item1 + ";" + pat.Value.Item2;
                    await file.WriteLineAsync(line);
                }
            }

            // Bases
            using (StreamWriter file = new(Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "custom_cohort_bases.csv")))
            {
                foreach (var pat in CustomCohort[Enums.ModelType.Bases])
                {
                    string line = pat.Key + ";" + pat.Value.Item1 + ";" + pat.Value.Item2;
                    await file.WriteLineAsync(line);
                }
            }


            // common
            using (StreamWriter file = new(Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "custom_data_files.csv")))
            {
                foreach (var str in CustomDataFiles)
                {
                    string line = str.Key + ";" + str.Value;
                    await file.WriteLineAsync(line);
                }
            }
        }

        public static Tuple<int, string> GetCustomCohortPatient(Enums.ModelType modelType, string name)
        {
            if (CustomCohort.Count == 0)
                LoadCustomCohort();

            if (CustomCohort[modelType].ContainsKey(name))
                return CustomCohort[modelType][name];

            return null;
        }

        public static Tuple<int, string> GetCustomCohortPatient(Enums.ModelType modelType, int patientId)
        {
            if (CustomCohort.Count == 0)
                LoadCustomCohort();

            if (CustomCohort.ContainsKey(modelType))
            {
                foreach (var pat in CustomCohort[modelType])
                {
                    if (pat.Value.Item1 == patientId)
                        return pat.Value;
                }
            }

            return null;
        }

        public static int AddCustomPatient(Enums.ModelType modelType, string name, string parameters)
        {
            int id = ++MaxCustomPatientId;

            if (!CustomCohort.ContainsKey(modelType))
                CustomCohort.Add(modelType, new Dictionary<string, Tuple<int, string>>());

            CustomCohort[modelType].Add(name, new Tuple<int, string>(id, parameters));

            SaveCustomCohort();

            return id;
        }

        public static bool HasCustomPatient(string name)
        {
            if (CustomCohort.Count == 0)
                LoadCustomCohort();

            foreach (var clss in CustomCohort)
            {
                if (clss.Value.ContainsKey(name))
                    return true;
            }

            return false;
        }

        public static void RemoveCustomPatient(string name)
        {
            if (CustomCohort.Count == 0)
                LoadCustomCohort();

            foreach (var clss in CustomCohort)
            {
                if (clss.Value.ContainsKey(name))
                    clss.Value.Remove(name);
            }
        }

        public static PatientRecord GetDefaultCohortPatient(Enums.ModelType modelType, int patientId)
        {
            if (DefaultCohort.ContainsKey(modelType))
            {
                foreach (var pat in DefaultCohort[modelType])
                {
                    if (pat.Id == patientId)
                        return pat;
                }
            }

            return null;
        }

        public static List<PatientRecord> GetDefaultCohort(Enums.ModelType modelType)
        {
            if (DefaultCohort.ContainsKey(modelType))
                return DefaultCohort[modelType];
            return null;
        }

        public static List<PatientRecord> GetCustomCohort(Enums.ModelType modelType)
        {
            if (CustomCohort.Count == 0)
                LoadCustomCohort();

            List<PatientRecord> recs = new List<PatientRecord>();

            foreach (var pat in CustomCohort[modelType])
            {
                PatientRecord rec = new PatientRecord { Id = pat.Value.Item1, Name = pat.Key, ParameterString = pat.Value.Item2, Age = 0, Diabetes = Enums.DiabetesType.Type1 };
                recs.Add(rec);
            }

            return recs;
        }

        private static string LoadDefaultCohortParams(Enums.ModelType modelType, int patientId)
        {
            var pat = GetDefaultCohortPatient(modelType, patientId);
            if (pat == null)
                return null;

            return pat.ParameterString;
        }

        private static string LoadUserStoredParams(Enums.ModelType modelType, int paramsId)
        {
            return "";
        }

        public static string GetDefaultParameters(Enums.ModelType modelType)
        {
            if (DefaultParameters.ContainsKey(modelType))
                return DefaultParameters[modelType];

            return "";
        }
    }
}
