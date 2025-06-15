using System;
using System.Drawing;
using System.Windows.Forms;
using GameLogic;
using GameLogic.Interface;
using GameLogic.Manager;

namespace GameInterface
{
    public partial class GameForm : Form
    {
        public TurnProcessor Processor { get; }

        //private readonly IGameContext _gameContext;
        //private readonly TurnProcessor _turnProcessor;

        private GameForm()
        {
            InitializeComponent(); 
        }

        public GameForm(TurnProcessor processor) : this()
        {
            Processor = processor;
        }

      
    }
}