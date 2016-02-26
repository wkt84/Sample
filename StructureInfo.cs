using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    public void Execute(ScriptContext context /*, System.Windows.Window window*/)
    {
        PlanSetup plan = context.PlanSetup;

        // Check if plan is opened
        if (plan == null)
            throw new ApplicationException("No plan opened.");

        // Check if structureset exists
        StructureSet structureSet = plan.StructureSet;
        if (structureSet == null)
            throw new ApplicationException("No StructureSet.");

        // Check if dose is calculated 
        bool IsCalculated = true;
        if (plan.Dose == null)
            IsCalculated = false;

        // Check if plan has PTV
        bool HasPTV = false;

        string message = "Structure Information\n";

        foreach (var structure in structureSet.Structures)
        {
            if (structure.DicomType == "PTV" && !structure.IsEmpty)
            {
                message += "\nPTV\n";
                HasPTV = true;

                // Get Volume
                message += string.Format("Volume: {0:f1} cm3\n", structure.Volume);

                // Get Center Position
                message += string.Format("Center Position x: {0:f1}, y: {1:f1}, z: {2:f1}\n", 
                    structure.CenterPoint.x, structure.CenterPoint.y, structure.CenterPoint.z);

                if (IsCalculated)
                {
                    // Get Mean Dose
                    DoseValue mean = plan.GetDVHCumulativeData(structure, 0, 0, 0.1).MeanDose;
                    message += string.Format("Mean dose: {0:f1}%\n", mean.Dose);

                    // Get D95
                    DoseValue D95 = plan.GetDoseAtVolume(structure, 95, 0, 0);
                    message += string.Format("D95 = {0:f1}%\n", D95.Dose);

                    // Get V100
                    double V100 = plan.GetVolumeAtDose(structure, new DoseValue(100, DoseValue.DoseUnit.Percent), 0);
                    message += string.Format("V100 = {0:f1}%\n", V100);
                }
            }
        }
        if (!HasPTV)
            message += "There is no PTV.\n";

        MessageBox.Show(message, "Structure Information");
    }
  }
}
