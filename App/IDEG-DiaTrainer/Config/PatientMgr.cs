using IDEG_DiaTrainer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace IDEG_DiaTrainer.Config
{
    public class PatientMgr
    {
        private static Dictionary<Enums.ModelType, List<PatientRecord>> DefaultCohort = new Dictionary<Enums.ModelType, List<PatientRecord>>{
            {
                Enums.ModelType.GCTv2, new List<PatientRecord>
                {
                    new PatientRecord{ Id = 1, Age = 35, Name = "Patient 1 (GCT)", Diabetes = Enums.DiabetesType.Type1, ParameterString = "0 0 0 0 0 0 0 3 1 3 50 8 4 0.5 1.4 1 5 0.1 0.144 0.144 0.001 0.1 1e-05 0.0001 1e-05 0 100 100 100 100 50 50 1 0.2 0.02 0.5 0.006944444444444445 0.003472222222222222 0.0006944444444444445 100 100 100 0 0 0 0 8 8 9 380 8 5 0.8 8 5 45 0.7 0.38519 0.38519 0.8 0.3 0.01 0.1 0.001 0 200 200 342 200 180 100 20 0.5 0.03 0.98 0.03055555555555556 0.04166666666666667 0.003472222222222222 500 500 500 500 500 500 200 10 10 20 1000 14 8 1.5 24 50 120 1 14.4 14.4 0.8 2 0.01 0.1 0.1 0.05 500 500 500 500 250 250 50 10 0.1 0.98 0.08333333333333333 0.2083333333333333 0.08333333333333333" }
                }
            },
            {
                Enums.ModelType.Bases, new List<PatientRecord>
                {
                    new PatientRecord{ Id = 1, Age = 35, Name = "Patient 1 (Bases)", Diabetes = Enums.DiabetesType.Type1, ParameterString = "0.05 0.05 0.25 -3 0.0001 0.001 0.001 -0.01041666666666667 -0.01041666666666667 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 0 3 -1 0 0 0 0.75 35 30 0.1 0.12 0.4 0.05000003348342125 0.3719896097390821 0.6354153897565117 2.999993222357193 0.006564797248322683 1.996356811430162 0.4999973999120048 -0.006545266105070348 0.04166666666666666 -0.2416477418341191 0.8999919619550478 0.2999140895910565 -0.7677592386117823 0.896819089935838 0.07740265634494041 1.734396789412239 0.1104689246382145 0.1998692676931243 0.9069822316606672 0.7708055640967928 0.0791070771355412 -0.2069864150421918 0.8999972674317342 0.2990692763364851 -0.9799724525162214 0.2690292461606456 0.1587600201027732 0.08266086950008318 0.3527266430188042 0.05001000967689229 1.221866855723666 0.1779289177514273 0.1911375812309319 1.999992743460905 0.8999892094201754 0.09112565913069011 -1.999986378677759 0.1000745306275651 0.2793590381243105 1.999999012143357 0.5050055725718575 0.08239173388474137 0.7904613503236841 4.346605219939125 0.9999629489229229 0.8224935209389294 49.97901016448262 4.175492375035708e-07 0.750186693676564 53.31300270262199 59.99985922484883 0.132158555668492 0.1211873026960382 0.8499999996781322 0.75 0.7 1 3 1 2 0.5 0.04166666666666666 0.04166666666666666 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 15 25 1 10 50 10 0.9 70 60 0.145 0.15 0.85" }
                }
            },
        };

        private static Dictionary<Enums.ModelType, string> DefaultParameters = new Dictionary<Enums.ModelType, string> {
            {
                Enums.ModelType.GCTv2, "0 0 0 0 0 0 0 3 1 3 50 8 4 0.5 1.4 1 5 0.1 0.144 0.144 0.001 0.1 1e-05 0.0001 1e-05 0 100 100 100 100 50 50 1 0.2 0.02 0.5 0.006944444444444445 0.003472222222222222 0.0006944444444444445 100 100 100 0 0 0 0 8 8 9 380 8 5 0.8 8 5 45 0.7 0.38519 0.38519 0.8 0.3 0.01 0.1 0.001 0 200 200 342 200 180 100 20 0.5 0.03 0.98 0.03055555555555556 0.04166666666666667 0.003472222222222222 500 500 500 500 500 500 200 10 10 20 1000 14 8 1.5 24 50 120 1 14.4 14.4 0.8 2 0.01 0.1 0.1 0.05 500 500 500 500 250 250 50 10 0.1 0.98 0.08333333333333333 0.2083333333333333 0.08333333333333333"
            },
            {
                Enums.ModelType.Bases, "0.05 0.05 0.25 -3 0.0001 0.001 0.001 -0.01041666666666667 -0.01041666666666667 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 -2 0.1 0.05 0 3 -1 0 0 0 0.75 35 30 0.1 0.12 0.4 0.576310608817704 0.6999671139739412 0.9999999753107783 0.08805490643441122 0.002410700007195571 1.28709583750478 0.4993109731814842 0.02171734455172674 -0.01041530618532603 1.645831577509386 0.8999990265538862 0.05000881801419771 -2 0.8372030032626945 0.05004419658540272 1.05382507732343 0.3260035275886074 0.2205129753028034 1.999999999128474 0.1004695223015807 0.2957620567177788 1.7294756829663 0.7680547662321416 0.2696118880720631 -1.536154345558184 0.1670188712657392 0.0500003196721768 -0.6669759773964558 0.6480213685132716 0.05000182106800625 -1.999999801630021 0.8999419466983288 0.1221544562348758 -1.999999733570519 0.126335861406715 0.297548460030097 -1.999999746784791 0.3718832522287376 0.05000081968127704 -1.999895598880423 0.3730077489961397 0.05000016711289706 0.01620135915762501 6.28162784940527 0.5163609673839118 9.562073514855413e-08 1.449264000190898e-05 0.003010545834454591 0.8982333955006198 35.00012991455316 59.99872528925555 0.1449794902921139 0.1499949691440464 0.8492556748564183 0.75 0.7 1 3 1 2 0.5 0.04166666666666666 0.04166666666666666 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 2 0.9 0.3 15 25 1 10 50 10 0.9 70 60 0.145 0.15 0.85"
            }
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
