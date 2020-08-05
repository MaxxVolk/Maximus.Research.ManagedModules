using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Maximus.Research.Modules
{
  [MonitoringModule(ModuleType.ReadAction)]
  [ModuleOutput(true)]
  public class SimpleWConfigProbeAction : ModuleBase<PropertyBagDataItem>
  {
    // tracking shutdown status
    private readonly object shutdownLock;
    private bool shutdown;
    private string TopLevelWorkflowName = "not loaded";

    private Guid instanceId = Guid.NewGuid();

    public SimpleWConfigProbeAction(ModuleHost<PropertyBagDataItem> moduleHost,
      XmlReader configuration,
      byte[] previousState) : base(moduleHost)
    {
      // don't need to check moduleHost != null => the base constructor does this
      // check configuration != null only if the module has configuration
      shutdownLock = new object();
      // load configuration here if any
      Log($"SimpleProbeAction class instance is created. Host process PID: {Process.GetCurrentProcess().Id}; Managed thread: {Thread.CurrentThread.ManagedThreadId}\r\n");
      if (configuration != null)
        LoadConfiguration(configuration);
      else
        throw new ArgumentNullException(nameof(configuration));
      Log($"Configuration is loaded.\r\n");
    }

    [InputStream(0)]
    public void OnNewDataItems(DataItemBase[] dataitems,
      bool logicallyGrouped,
      DataItemAcknowledgementCallback acknowledgeCallback,
      object acknowledgedState,
      DataItemProcessingCompleteCallback completionCallback,
      object completionState)
    {
      Log($"Entering {nameof(OnNewDataItems)}; Managed thread: {Thread.CurrentThread.ManagedThreadId}\r\n");
      Log($"Received {(dataitems == null ? 0 : dataitems.Length)} inbound data items of {(dataitems == null && dataitems.Length >=1 ? "<unknown>" : dataitems[0].DataItemTypeName)} type.\r\n");
      lock (shutdownLock)
      {
        // don't start if shutdown is in progress
        if (shutdown)
          return;
        Log($"No shutdown in progress.\r\n");
        PropertyBagDataItem[] ReturningResults = null;
        try
        {
          // put the actual monitoring code here: ReturningResults = <get result code>
          Log($"Competed output data item.\r\n");
        }
        catch (Exception e)
        {
          Log($"Error while getting ouput data: {e.Message}.\r\n");
        }
        if (ReturningResults != null && ReturningResults.Length != 0)
        {
          // send data back to SCOM Agent, if any
          if (ReturningResults.Length == 1)
            ModuleHost.PostOutputDataItem(ReturningResults[0]);
          else
            ModuleHost.PostOutputDataItems(ReturningResults, logicallyGrouped);
          Log($"Output data item(s) posted.\r\n");
        }
        // completed the current run, ask for next
        ModuleHost.RequestNextDataItem();
        Log($"Next data item requested.\r\n");
      }
    }

    public override void Shutdown()
    {
      Log($"Entering {nameof(Shutdown)}; Managed thread: {Thread.CurrentThread.ManagedThreadId}.\r\n");
      lock (shutdownLock)
      {
        shutdown = true;
        Log($"{nameof(shutdown)} is set to True.\r\n");
      }
    }

    public override void Start()
    {
      Log($"Entering {nameof(Start)}; Managed thread: {Thread.CurrentThread.ManagedThreadId}.\r\n");
      lock (shutdownLock)
      {
        if (shutdown)
          return;
        // Request the very first data item
        ModuleHost.RequestNextDataItem();
        Log($"First data item requested.\r\n");
      }
    }

    private void LoadConfiguration(XmlReader configuration)
    {
      XmlDocument fullConfig = new XmlDocument();
      fullConfig.Load(configuration);
      TopLevelWorkflowName = fullConfig.GetElementsByTagName("TopLevelWorkflowName")[0].InnerText;
    }

    private void Log(string msg)
    {
      int maxAttempts = 10;
      while (true)
        try
        {
          maxAttempts--;
          if (maxAttempts < 0)
            break;
          File.AppendAllText(@"C:\Temp\SimpleWConfigProbeAction.txt", $"[{instanceId}]:[{TopLevelWorkflowName}]:[{DateTime.Now.ToString("HH:mm:ss")}]: {msg}");
          break;
        }
        catch
        {
          Thread.Sleep(1);
          continue;
        }
    }
  }
}
