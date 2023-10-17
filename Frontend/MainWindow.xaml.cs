using Backend;
using Backend.Data;
using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Frontend
{
    public partial class MainWindow : Window
    {
        private Timer? _timer;
        private ulong _s;
        private bool _canCount;

        private Settings _settings;
        private Grid _currentGrid;
        private Exercise? _currentExercise;
        private int previousNode;

        public MainWindow()
        {
            InitializeComponent();
            _currentGrid = MainGrid;
            _currentExercise = null;
            _canCount = false;
            StartAppTimer();
        }

        private void FixedUpdate()
        {
            if (_currentExercise is not null && _currentExercise.CurrentNode is not null && _currentExercise.CurrentNode.Elapsed > _currentExercise.CurrentNode.Length)
            {
                if (_currentExercise.NodePointer < _currentExercise.ExerciseNodes.Count - 1)
                {
                    _currentExercise.Next();
                }
                else
                {
                    _canCount = false;
                    _currentGrid = TimerEndGrid;
                }
            }

            UpdateUI();
        }

        private void UpdateUI() 
        {
            MainGrid.Visibility = (_currentGrid.Name == MainGrid.Name) ? Visibility.Visible : Visibility.Collapsed;
            SettingsGrid.Visibility = (_currentGrid.Name == SettingsGrid.Name) ? Visibility.Visible : Visibility.Collapsed;
            TimerStartGrid.Visibility = (_currentGrid.Name == TimerStartGrid.Name) ? Visibility.Visible : Visibility.Collapsed;
            TimerGrid.Visibility = (_currentGrid.Name == TimerGrid.Name) ? Visibility.Visible : Visibility.Collapsed;
            TimerEndGrid.Visibility = (_currentGrid.Name == TimerEndGrid.Name) ? Visibility.Visible : Visibility.Collapsed;

            if (_currentExercise is null) return;
            if (_currentExercise.CurrentNode is null) return;

            if (_currentExercise.NodePointer == 1 && previousNode == 0)
            {
                Task.Factory.StartNew(() => 
                {
                    var uri = new Uri(@"C:\Users\orosz\source\repos\oroszpeter97\kettlebell-counter\Frontend\whistle.mp3", UriKind.RelativeOrAbsolute);
                    var player = new MediaPlayer();

                    player.Open(uri);
                    player.Play(); 
                }); 
            }
            previousNode = _currentExercise.NodePointer;

            switch (_currentGrid.Name)
            {
                case "TimerStartGrid":
                    TextBlockExerciseNameStart.Text = _currentExercise.Name;
                    break;
                case "TimerGrid":
                    TextBlockExerciseName.Text = _currentExercise.Name;

                    TextBlockExerciseResults.Text = "";
                    for (int i = 0; i < _currentExercise.NodePointer; i++)
                    {
                        if (_currentExercise.ExerciseNodes[i].Name == "Rest") continue;
                        TextBlockExerciseResults.Text = TextBlockExerciseResults.Text + "\n" + _currentExercise.ExerciseNodes[i].Name + ": " + _currentExercise.ExerciseNodes[i].Reps;
                    }
                    TextBlockExerciseResults.Visibility = 
                        (_currentExercise.CurrentNode.Name == "Rest") ? 
                        TextBlockExerciseResults.Visibility = Visibility.Visible : TextBlockExerciseResults.Visibility = Visibility.Collapsed;

                    TextBlockExerciseNodeName.Text = _currentExercise.CurrentNode.Name;

                    TextBlockExcerciseRepsCounter.Text = _currentExercise.CurrentNode.Reps.ToString();
                    TextBlockExcerciseRepsCounter.Visibility =
                        (_currentExercise.CurrentNode.Name == "Rest") ?
                        Visibility.Collapsed  : Visibility.Visible;

                    if (_settings.Timing == Timing.Countdown)
                    {
                        ulong invertedTime = _currentExercise.CurrentNode.Length - _currentExercise.CurrentNode.Elapsed;
                        ulong minutes = invertedTime / 60;
                        ulong seconds = invertedTime % 60;
                        if (seconds < 10)
                        {
                            TextBlockExcerciseTimeCounter.Text = minutes.ToString() + " : 0" + seconds.ToString();
                        }
                        else
                        {
                            TextBlockExcerciseTimeCounter.Text = minutes.ToString() + " : " + seconds.ToString();
                        }
                    }
                    else
                    {
                        ulong minutes = _currentExercise.CurrentNode.Elapsed / 60;
                        ulong seconds = _currentExercise.CurrentNode.Elapsed % 60;
                        if (seconds < 10)
                        {
                            TextBlockExcerciseTimeCounter.Text = minutes.ToString() + " : 0" + seconds.ToString();
                        }
                        else
                        {
                            TextBlockExcerciseTimeCounter.Text = minutes.ToString() + " : " + seconds.ToString();
                        }
                    }
                    TextBlockExcerciseTimeCounter.Foreground =
                        (_currentExercise.CurrentNode.Name == "Rest") ?
                        new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White);
                    break;
                case "TimerEndGrid":
                    TextBlockExerciseNameEnd.Text = _currentExercise.Name;
                    TextBlockExerciseResultsEnd.Text = "";
                    for(int i = 0; i < _currentExercise.ExerciseNodes.Count; i++)
                    {
                        if (_currentExercise.ExerciseNodes[i].Name == "Rest") continue;
                        TextBlockExerciseResultsEnd.Text = TextBlockExerciseResultsEnd.Text + "\n" + _currentExercise.ExerciseNodes[i].Name + ": " + _currentExercise.ExerciseNodes[i].Reps;
                    }
                    break;
                default:
                    break;
            }
        }

        #region Exercise Button Clilcks
        private void Pentathlon_Click(object sender, RoutedEventArgs e) 
        {
            List<ExerciseNode> exerciseNodes = new List<ExerciseNode>
            {
                new ExerciseNode(0, "Rest", 5, 0),
                new ExerciseNode(1, "Clean", 360, 120),
                new ExerciseNode(2, "Rest", 300, 0),
                new ExerciseNode(1, "Clean&Press", 360, 60),
                new ExerciseNode(2, "Rest", 300, 0),
                new ExerciseNode(1, "Half Snatch", 360, 108),
                new ExerciseNode(2, "Rest", 300, 0),
                new ExerciseNode(1, "Clean", 360, 120)
            };

            Exercise pentathlon = new Exercise("Pentathlon", exerciseNodes);

            _currentExercise = pentathlon;
            _currentGrid = TimerStartGrid;
            UpdateUI();
        }
        private void HalfPentathlon_Click(object sender, RoutedEventArgs e) 
        {
            List<ExerciseNode> exerciseNodes = new List<ExerciseNode>
            {
                new ExerciseNode(0, "Rest", 5, 0),
                new ExerciseNode(1, "Clean", 120, 60),
                new ExerciseNode(2, "Rest", 120, 0),
                new ExerciseNode(1, "Clean&Press", 120, 30),
                new ExerciseNode(2, "Rest", 120, 0),
                new ExerciseNode(1, "Half Snatch", 120, 54),
                new ExerciseNode(2, "Rest", 120, 0),
                new ExerciseNode(1, "Clean", 120, 60)
            };

            Exercise halfPentathlon = new Exercise("Half Pentathlon", exerciseNodes);

            _currentExercise = halfPentathlon;
            _currentGrid = TimerStartGrid;
            UpdateUI();
        }
        private void SprintThree_Click(object sender, RoutedEventArgs e) 
        {
            List<ExerciseNode> exerciseNodes = new List<ExerciseNode>
            {
                new ExerciseNode(0, "Rest", 5, 0),
                new ExerciseNode(1, "Sprint - 3'", 180, int.MaxValue)
            };

            Exercise sprint = new Exercise("Sprint", exerciseNodes);

            _currentExercise = sprint;
            _currentGrid = TimerStartGrid;
            UpdateUI();
        }
        private void SprintFive_Click(object sender, RoutedEventArgs e) 
        {
            List<ExerciseNode> exerciseNodes = new List<ExerciseNode>
            {
                new ExerciseNode(0, "Rest", 5, 0),
                new ExerciseNode(1, "Sprint", 300, int.MaxValue)
            };

            Exercise sprint = new Exercise("Sprint - 5'", exerciseNodes);

            _currentExercise = sprint;
            _currentGrid = TimerStartGrid;
            UpdateUI();
        }
        private void SprintTen_Click(object sender, RoutedEventArgs e) 
        {
            List<ExerciseNode> exerciseNodes = new List<ExerciseNode>
            {
                new ExerciseNode(0, "Rest", 5, 0),
                new ExerciseNode(1, "Sprint", 600, int.MaxValue)
            };

            Exercise sprint = new Exercise("Sprint - 10'", exerciseNodes);

            _currentExercise = sprint;
            _currentGrid = TimerStartGrid;
            UpdateUI();
        }
        private void SprintTwelve_Click(object sender, RoutedEventArgs e) 
        {
            List<ExerciseNode> exerciseNodes = new List<ExerciseNode>
            {
                new ExerciseNode(0, "Rest", 5, 0),
                new ExerciseNode(1, "Sprint", 720, int.MaxValue)
            };

            Exercise sprint = new Exercise("Sprint - 12'", exerciseNodes);

            _currentExercise = sprint;
            _currentGrid = TimerStartGrid;
            UpdateUI();
        }
        private void SprintThirty_Click(object sender, RoutedEventArgs e) 
        {
            List<ExerciseNode> exerciseNodes = new List<ExerciseNode>
            {
                new ExerciseNode(0, "Rest", 5, 0),
                new ExerciseNode(1, "Sprint", 1800, int.MaxValue)
            };

            Exercise sprint = new Exercise("Sprint - 30'", exerciseNodes);

            _currentExercise = sprint;
            _currentGrid = TimerStartGrid;
            UpdateUI();
        }
        private void SprintSixty_Click(object sender, RoutedEventArgs e) 
        {
            List<ExerciseNode> exerciseNodes = new List<ExerciseNode>
            {
                new ExerciseNode(0, "Rest", 5, 0),
                new ExerciseNode(1, "Sprint", 3600, int.MaxValue)
            };

            Exercise sprint = new Exercise("Sprint - 60'", exerciseNodes);

            _currentExercise = sprint;
            _currentGrid = TimerStartGrid;
            UpdateUI();
        }
        private void Custom_Click(object sender, RoutedEventArgs e) 
        {
            List<ExerciseNode> exerciseNodes = new List<ExerciseNode>
            {
                new ExerciseNode(0, "Rest", 5, 10),
                new ExerciseNode(1, "Test Node 1", 10, 10),
                new ExerciseNode(2, "Rest", 5, 10),
                new ExerciseNode(3, "Test Node 2", 10, 10)
            };

            Exercise pentathlon = new Exercise("Custom", exerciseNodes);

            _currentExercise = pentathlon;
            _currentGrid = TimerStartGrid;
            UpdateUI();
        }
        #endregion

        #region Misc Button Clicks
        private void Settings_Click(object sender, RoutedEventArgs e) 
        {
            _currentGrid = SettingsGrid;
            UpdateUI();
        }
        private void SaveAndReturn_Click(object sender, RoutedEventArgs e) 
        {
            bool soundToggle = false;
            Timing timing = Timing.Countdown;
            if (SoundToggle.IsChecked is not null)
            {
                soundToggle = (bool)SoundToggle.IsChecked;
            }
            if (TimingToggle.IsChecked is not null)
            {
                switch ((bool)TimingToggle.IsChecked)
                {
                    case true: timing = Timing.Stopwatch; break;
                    case false: timing = Timing.Countdown; break;
                }
            }

            _settings = new Settings(soundToggle, timing);

            _currentGrid = MainGrid;

            UpdateUI();
        }
        private void New_Click(object sender, RoutedEventArgs e) 
        {
            if (_currentExercise is null) return;

            _currentExercise.Reset();
            _currentGrid = TimerGrid;
            _canCount = true;
            UpdateUI();
        }
        private void Close_Click(object sender, RoutedEventArgs e) 
        {
            _currentExercise = null;
            _currentGrid = MainGrid;
            UpdateUI();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _canCount = true;
            _currentGrid = TimerGrid;
            UpdateUI();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _canCount = false;

            _currentExercise = null;
            _currentGrid = MainGrid;
            UpdateUI();
        }
        #endregion

        #region App Core Functionality
        private void StartAppTimer()
        {
            _s = 0;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            _timer = new Timer(1000);
            _timer.Elapsed += (sender, e) => TimerElapsed(sender, e, dispatcher);
            _timer.Start();
        }

        private void TimerElapsed(object? sender, ElapsedEventArgs e, Dispatcher dispatcher)
        {
            dispatcher.Invoke(() =>
            {
                if (_currentExercise is not null && _currentExercise.CurrentNode is not null && _canCount)
                    _currentExercise.CurrentNode.Elapsed++;
                _s++;
                FixedUpdate();
            });
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key pressedKey = e.Key;
            if (_currentExercise is null || _currentExercise.CurrentNode is null) return;

            if((e.Key == Key.Space || e.Key == Key.Up) && _currentExercise.CurrentNode.Reps < _currentExercise.CurrentNode.MaxReps)
            {
                e.Handled = true;
                _currentExercise.CurrentNode.Reps++;
                UpdateUI();
            }
            else if(e.Key == Key.Down && _currentExercise.CurrentNode.Reps > 0)
            {
                e.Handled = true;
                _currentExercise.CurrentNode.Reps--;
                UpdateUI();
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var toggleButton = (ToggleButton)thumb.TemplatedParent;
            toggleButton.IsChecked = e.HorizontalChange > 0;
        }

        private void Thumb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var thumb = (Thumb)sender;
            var toggleButton = (ToggleButton)thumb.TemplatedParent;
            toggleButton.IsChecked = !toggleButton.IsChecked;
        }

        private void Thumb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var thumb = (Thumb)sender;
            var toggleButton = (ToggleButton)thumb.TemplatedParent;
            toggleButton.IsChecked = !toggleButton.IsChecked;
        }
        #endregion
    }
}
