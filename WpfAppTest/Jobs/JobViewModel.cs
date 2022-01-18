using EconomicCalculator;
using EconomicCalculator.DTOs.Jobs;
using EconomicCalculator.DTOs.Products.ProductTags;
using Editor.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Editor.Jobs
{
    public class JobViewModel : INotifyPropertyChanged
    {
        private JobDTO job;
        private DTOManager manager;
        private JobModel model;

        private ICommand shiftToJob;
        private ICommand removeFromJob;
        private ICommand commitJob;
        private string _selectedProc;
        private string _selectedProcInJob;

        public JobViewModel(JobDTO job)
        {
            this.job = job;

            manager = DTOManager.Instance;

            model = new JobModel(job);

            AvailableProcesses = new ObservableCollection<string>
                (manager.Processes.Values
                    .Where(x => !job.ProcessNames.Any(y => y == x.GetName()))
                    .Select(x => x.GetName()));
            AvailableLabors = new ObservableCollection<string>(
                manager.Products.Values
                .Where(x => x.ContainsTag(ProductTag.Service))
                .Select(x => x.GetName())
                );
            AvailableSkills = new ObservableCollection<string>(manager.Skills.Values.Select(x => x.Name));
        }

        #region Commands

        public ICommand ShiftToJob
        {
            get
            {
                if (shiftToJob == null)
                {
                    shiftToJob = new RelayCommand(
                        param => ShiftProcessToJob());
                }
                return shiftToJob;
            }
        }

        public ICommand RemoveFromJob
        {
            get
            {
                if (removeFromJob == null)
                {
                    removeFromJob = new RelayCommand(
                        param => RemoveProcessFromJob());
                }
                return removeFromJob;
            }
        }

        public ICommand CommitJob
        {
            get
            {
                if (commitJob == null)
                {
                    commitJob = new RelayCommand(
                        param => CommitJobToMemory());
                }
                return commitJob;
            }
        }

        #endregion Commands

        #region CommandFunctions

        public void ShiftProcessToJob()
        {
            if (SelectedProc != null)
            {
                InJob.Add(SelectedProc);
                AvailableProcesses.Remove(SelectedProc);
            }
        }

        public void RemoveProcessFromJob()
        {
            if (SelectedProcInJob != null)
            {
                AvailableProcesses.Add(SelectedProcInJob);
                InJob.Remove(SelectedProcInJob);
            }
        }

        public void CommitJobToMemory()
        {
            // check name
            if (string.IsNullOrEmpty(model.Name))
            {
                MessageBox.Show("Name must exist and be at least 3 character's long.");
                return;
            }

            // check labor
            if (string.IsNullOrEmpty(model.Labor))
            {
                MessageBox.Show("A labor must be selected.");
                return;
            }

            // check Skill
            if (string.IsNullOrEmpty(model.Skill))
            {
                MessageBox.Show("A Skill must be Selected.");
                return;
            }

            // check jobs
            // no processes required.

            // save
            var newJob = new JobDTO
            {
                Id = job.Id,
                Name = model.Name,
                VariantName = model.VariantName,
                Labor = model.Labor,
                LaborId = manager.GetProductByFullName(model.Labor).Id,
                Skill = model.Skill,
                SkillId = manager.GetSkillByName(model.Skill).Id
            };

            foreach (var proc in model.Processes)
            {
                newJob.ProcessNames.Add(proc);
                newJob.ProcessIds.Add(manager.GetProcessByName(proc).Id);
            }

            manager.Jobs[newJob.Id] = newJob;
        }

        #endregion CommandFunctions

        #region Lists

        public ObservableCollection<string> InJob
        {
            get
            {
                return model.Processes;
            }
        }

        public ObservableCollection<string> AvailableProcesses
        {
            get;
            set;
        }

        public ObservableCollection<string> AvailableLabors { get; set; }

        public ObservableCollection<string> AvailableSkills { get; set; }

        #endregion Lists

        #region Properties

        public string Name
        {
            get
            {
                return model.Name;
            }
            set
            {
                if (model.Name != value)
                {
                    model.Name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string VariantName
        {
            get
            {
                return model.VariantName;
            }
            set
            {
                if (model.VariantName != value)
                {
                    model.VariantName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedLabor
        {
            get
            {
                return model.Labor;
            }
            set
            {
                if (model.Labor != value)
                {
                    model.Labor = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedSkill
        {
            get
            {
                return model.Skill;
            }
            set
            {
                if (model.Skill != value)
                {
                    model.Skill = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedProcInJob
        {
            get
            {
                return _selectedProcInJob;
            }
            set
            {
                if (_selectedProcInJob != value)
                {
                    _selectedProcInJob = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedProc
        {
            get
            {
                return _selectedProc;
            }
            set
            {
                if (_selectedProc != value)
                {
                    _selectedProc = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion Properties

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?
                .Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
