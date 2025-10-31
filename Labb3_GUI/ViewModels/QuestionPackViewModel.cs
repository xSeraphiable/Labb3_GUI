using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Labb3_GUI.Models;

namespace Labb3_GUI.ViewModels;

internal class QuestionPackViewModel : ViewModelBase
{
    private readonly QuestionPack _model;
    public QuestionPackViewModel(QuestionPack model)
    {
        _model = model;
        Questions = new ObservableCollection<Question>(_model.Questions);
        Questions.CollectionChanged += Questions_CollectionChanged;
    }

    private void Questions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            foreach (Question q in e.NewItems) _model.Questions.Add(q);

        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            foreach (Question q in e.OldItems) _model.Questions.Remove(q);

        if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null && e.OldItems != null)
            _model.Questions[e.OldStartingIndex] = (Question)e.NewItems[0];

        if (e.Action == NotifyCollectionChangedAction.Reset)
            _model.Questions.Clear();

    }

    public ObservableCollection<Question> Questions { get; set; }
    public string Name
    {
        get => _model.Name;
        set
        {
            _model.Name = value;
            RaisePropertyChanged("Hej");

        }
    }

    public Difficulty difficulty
    {
        get => difficulty; set
        {
            _model.Difficulty = value;
            RaisePropertyChanged();
        }
    }

    public int TimeLimitInSeconds
    {
        get => TimeLimitInSeconds;
        set
        {
            _model.TimeLimitInSeconds = value;
            RaisePropertyChanged();
        }
    }


}
