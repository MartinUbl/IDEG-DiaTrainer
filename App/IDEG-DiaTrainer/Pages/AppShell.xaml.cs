using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace IDEG_DiaTrainer.Pages;

public partial class AppShell : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();
    public ICommand HelpCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
        BindingContext = this;
    }

    void RegisterRoutes()
    {
        Routes.Add("scenarioselection", typeof(ScenarioSelectionPage));
        Routes.Add("patientselection", typeof(PatientSelectionPage));
        Routes.Add("simulation", typeof(SimulationPage));
        Routes.Add("patientpersonalization", typeof(PatientPersonalizationPage));

        foreach (var item in Routes)
        {
            Routing.RegisterRoute(item.Key, item.Value);
        }
    }
}
