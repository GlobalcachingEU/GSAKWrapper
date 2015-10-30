using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.FlowSequences
{
    public class Manager
    {
        private static Manager _uniqueInstance = null;
        private static object _lockObject = new object();

        public ObservableCollection<FlowSequence> FlowSequences { get; set; }

        public Manager()
        {
#if DEBUG
            if (_uniqueInstance != null)
            {
                //you used the wrong binding
                System.Diagnostics.Debugger.Break();
            }
#endif
            FlowSequences = new ObservableCollection<FlowSequence>();
            loadFlowSeqeunces();
        }

        public static Manager Instance
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_uniqueInstance == null)
                        {
                            _uniqueInstance = new Manager();
                        }
                    }
                }
                return _uniqueInstance;
            }
        }

        private void loadFlowSeqeunces()
        {
            //todo
        }

        public async Task RunFowSequence(FlowSequence fs)
        {
            await Task.Run(() =>
            {
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    //todo
                    sw.Stop();
                    ApplicationData.Instance.StatusText = string.Format(Localization.TranslationManager.Instance.Translate("FlowSequenceFinishedIn") as string, fs.Name, sw.Elapsed.TotalSeconds.ToString("0.0"));
                }
                catch (Exception e)
                {
                    sw.Stop();
                    ApplicationData.Instance.StatusText = string.Format("{0}: {1}", Localization.TranslationManager.Instance.Translate("Error"), e.Message);
                }
            });
        }

    }
}
