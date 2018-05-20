using ShipsWar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipsWar
{
    public partial class Form1 : Form
    {
        public ModelGame Game;
        public FieldSetup Setup;
        TableLayoutPanel PlayerField;
        TableLayoutPanel Ships;
        Button FinishButton;
        public Dictionary<int, Label> AmountOfShipsLabels = new Dictionary<int, Label>();


        public TableLayoutPanel MakeField(Point coordinate)
        {
            var table = new TableLayoutPanel();
            table.Size = new Size(ModelGame.Size * 40, ModelGame.Size * 40);
            table.Location = coordinate;
            for (int i = 0; i < ModelGame.Size; i++)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / ModelGame.Size));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / ModelGame.Size));
            }
            for (int column = 0; column < ModelGame.Size; column++)
                for (int row = 0; row < ModelGame.Size; row++)
                {
                    var iRow = row;
                    var iColumn = column;
                    var button = new Button();
                    button.Dock = DockStyle.Fill;
                    button.Click += (sender, args) => Setup.PutShip(new Point(iColumn, iRow));
                    button.Click += (sender, args) => UpdateForm();
                    table.Controls.Add(button, iColumn, iRow);
                }
            return table;
        }

        public TableLayoutPanel MakeShips(Point coordinate)
        {
            MakeShipStatLabels();
            var shipTable = new TableLayoutPanel();
            shipTable.Size = new Size(40, 160);
            shipTable.Location = coordinate;
            for (int i = 0; i < 4; i++)
                shipTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 4));
            shipTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            for (int i = 0; i < 4; i++)
            {
                var iRow = i;
                var button = new Button();
                button.Dock = DockStyle.Fill;
                button.Click += (sender, args) => Setup.ChooseShip(iRow+1);
                button.Click += (sender, args) => UpdateForm();
                shipTable.Controls.Add(button, 0, iRow);
            }
            return shipTable;

        }

        public Button RotateButton(Point coordinate)
        {
            var button = new Button();
            button.Location = coordinate;
            button.Size = new System.Drawing.Size(100, 100);
            button.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            button.Text = "rotate ---";
            button.Click += (sender, args) => Setup.Angle = (Setup.Angle == 90)? 0 : 90;
            button.Click += (sender, args) => button.Text = (Setup.Angle == 90) ? "rotate |" : "rotate ---"; 
            return button;
        }


        public Button ResetButton(Point coordinate)
        {
            var button = new Button();
            button.Location = coordinate;
            button.Size = new System.Drawing.Size(100, 100);
            button.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            button.Text = "RESET";
            button.Click += (sender, args) => Setup.Reset();
            button.Click += (sender, args) => UpdateForm();
            return button;
        }

        public void MakeFinishButton(Point coordinate)
        {
            FinishButton = new Button();
            FinishButton.Location = coordinate;
            FinishButton.BackColor = Color.LightGray;
            FinishButton.Size = new System.Drawing.Size(100, 100);
            FinishButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            FinishButton.Text = "Start";
            FinishButton.Click += (sender, args) => this.Hide();
            FinishButton.Click += (sender, args) => { var form2 = new Form2(Game); form2.Show(this); };
        }

        public Label MakeShipNameLabel(Point position, string text)
        {
            var label = new Label();
            label.Location = position;
            label.Size = new Size(80, 20);
            label.BackColor = Color.LightGray;
            label.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Text = text;
            return label;
        }

        public Label MakeCoordinateLabel(Point position, string text)
        {
            var label = new Label();
            label.Location = position;
            label.Size = new Size(40, 40);
            label.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Text = text;
            return label;
        }

        public void MakeShipStatLabels()
        {
            for (int i = 1; i < 5; i++)
            {
                var label = new Label();
                label.Location = new Point(560, 100 + 40 * (i-1));
                label.Size = new Size(40, 40);
                label.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Text = Setup.AmountOfShips[i].ToString();
                AmountOfShipsLabels[i] = label;
            }
        }

        public void UpdateForm()
        {
            foreach (var label in AmountOfShipsLabels)
                label.Value.Text = Setup.AmountOfShips[label.Key].ToString();
            bool isCompleted = true; 
            foreach (var shipType in Setup.AmountOfShips)
                if (shipType.Value != 0)
                    isCompleted = false;
            if (isCompleted)
            {
                FinishButton.BackColor = Color.LightGreen;
                FinishButton.Enabled = true;
            }
            else
            {
                FinishButton.BackColor = Color.LightGray;
                FinishButton.Enabled = false;
            }
            foreach (var ship in Setup.ActiveShip)
            {
                if (ship.Value)
                    ((Button)Ships.GetControlFromPosition(0, ship.Key-1)).BackColor = Color.Green;
                else
                    ((Button)Ships.GetControlFromPosition(0, ship.Key-1)).BackColor = Color.LightGray;
            }
            for (int column = 0; column < ModelGame.Size; column++)
                for (int row = 0; row < ModelGame.Size; row++)
                {
                    switch(Game.FirstPlayer.Map.Location[column, row])
                    {
                        case State.Ship:
                            ((Button)PlayerField.GetControlFromPosition(column, row)).BackColor = Color.Green;
                            break;
                        case State.NearShip:
                            ((Button)PlayerField.GetControlFromPosition(column, row)).BackColor = Color.LightGray;
                            break;
                        case State.Empty:
                            ((Button)PlayerField.GetControlFromPosition(column, row)).BackColor = Color.White;
                            break;
                    }
                }
        }

        public Form1(ModelGame game)
        {
            this.Game = game;
            this.Setup = new FieldSetup(Game);
            this.FormClosing+= (sender, args) => Application.Exit();
            PlayerField = MakeField(new Point(100, 100));
            Ships = MakeShips(new Point(600, 100));
            MakeFinishButton(new Point(800, 100));
            UpdateForm();
            Controls.Add(PlayerField);
            Controls.Add(Ships);
            Controls.Add(FinishButton);
            Controls.Add(RotateButton(new Point(800, 250)));
            Controls.Add(ResetButton(new Point(800, 400)));
            Controls.Add(MakeShipNameLabel(new Point(650, 105), "1-Cell"));
            Controls.Add(MakeShipNameLabel(new Point(650, 145), "2-Cell"));
            Controls.Add(MakeShipNameLabel(new Point(650, 185), "3-Cell"));
            Controls.Add(MakeShipNameLabel(new Point(650, 225), "4-Cell"));
            foreach (var stateLabel in AmountOfShipsLabels)
                Controls.Add(stateLabel.Value);
            for (int i = 0; i < ModelGame.Size; i++)
                Controls.Add(MakeCoordinateLabel(new Point(60, 100+i*40), (i + 1).ToString()));
            for (int i = 0; i < ModelGame.Size; i++)
                Controls.Add(MakeCoordinateLabel(new Point(100 + i * 40, 60), Convert.ToString((char)(i+65))));
            var backSound = new SoundPlayer();
            backSound.SoundLocation = @"..\..\backMusic.wav";
            backSound.Load();
            backSound.PlayLooping();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
