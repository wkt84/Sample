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

        // Check if dose is calculated 
        bool IsCalculated = true;
        if (plan.Dose == null)
            IsCalculated = false;

        string message = "Field Information\n";

        foreach (var beam in plan.Beams)
        {
            message += string.Format("\n{0}\n", beam.Id);

            // Get Gantry Angle (start angle for arc field)
            message += string.Format("Gantry Angle: {0:f1}\n", beam.ControlPoints.ElementAt(0).GantryAngle);

            // Get Isocenter Position in mm
            message += string.Format("Isocenter x: {0:f2}, y: {1:f2}, z: {2:f2}\n",
                beam.IsocenterPosition.x, beam.IsocenterPosition.y, beam.IsocenterPosition.z);

            // Get MLC Model
            if (beam.MLC != null)
                message += string.Format("MLC Model: {0}\n", beam.MLC.Model);
            else
                message += "No MLC.\n";

            // Get MU if dose is calculated
            if (IsCalculated && !beam.IsSetupField)
                message += string.Format("MU: {0:f1}\n", beam.Meterset.Value);           

        }

        MessageBox.Show(message, "Field Information");
    }
  }
}
