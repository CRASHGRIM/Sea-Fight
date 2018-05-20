
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Media;

namespace ShipsWar
{
    partial class Form2 : Form
    {
        ModelGame Game;
        TableLayoutPanel PlayerField;
        TableLayoutPanel OpponentField;
        

        public TableLayoutPanel MakeField(Point coordinate, bool isPlayerField)
        {
            var shootSound = new System.Windows.Media.MediaPlayer();
            shootSound.Open(new Uri(@"..\..\shootSound.wav", UriKind.Relative));
            var table = new TableLayoutPanel();
            table.Size = new Size(400, 400);
            table.Location = new System.Drawing.Point(coordinate.X, coordinate.Y);
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
                    if (!isPlayerField)
                    {
                        button.Click += (sender, args) => shootSound.Play();
                        button.Click += (sender, args) => shootSound.Position=new TimeSpan(0);
                        button.Click += (sender, args) => { Game.FirstPlayer.Shoot(new Point(iColumn, iRow)); };
                    }
                    table.Controls.Add(button, iColumn, iRow);
                }
            return table;
        }

       
        public Button makeExitButton(Point coordinates)
        {
            var button = new Button();
            button.Location = coordinates;
            button.Size = new Size(new Point(100, 100));
            button.Text = "EXIT";
            button.BackColor = Color.LightGray;
            button.Click += (sender, args) => Application.Exit();
            return button;
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


        public Form2(ModelGame game)
        {
            this.FormClosing += (sender, args)=> Application.Exit();
            this.ClientSize = new Size(1000, 1000);
            this.Game = game;
            game.FirstPlayer.Map.Location = game.FirstPlayer.PrepareMap(game.FirstPlayer.Map.Location);
            PlayerField = MakeField(new Point(100, 100), true);
            Controls.Add(PlayerField);
            OpponentField = MakeField(new Point(600, 100), false);
            Controls.Add(OpponentField);
            var exitButton = makeExitButton(new Point(1100, 100));
            Controls.Add(exitButton);
            var backSound = new SoundPlayer();
            backSound.SoundLocation = @"..\..\backMusic.wav";
            backSound.Load();
            backSound.PlayLooping();
            for (int i = 0; i < ModelGame.Size; i++)
                Controls.Add(MakeCoordinateLabel(new Point(60, 100 + i * 40), (i + 1).ToString()));
            for (int i = 0; i < ModelGame.Size; i++)
                Controls.Add(MakeCoordinateLabel(new Point(100 + i * 40, 60), Convert.ToString((char)(i + 65))));
            for (int i = 0; i < ModelGame.Size; i++)
                Controls.Add(MakeCoordinateLabel(new Point(560, 100 + i * 40), (i + 1).ToString()));
            for (int i = 0; i < ModelGame.Size; i++)
                Controls.Add(MakeCoordinateLabel(new Point(600 + i * 40, 60), Convert.ToString((char)(i + 65))));


            game.FirstPlayer.SetPos += UpdatePointFirst;

            game.SecondPlayer.SetPos += UpdatePointOpponent;
            FUpdate();
        }

        public void UpdatePointFirst(Point point, State state)
        {
            ((Button)PlayerField.GetControlFromPosition(point.X, point.Y)).BackColor = StateToColor(state);
            if (state != State.Shooting && state != State.KilledShip)
            {
                for (int column = 0; column < ModelGame.Size; column++)
                    for (int row = 0; row < ModelGame.Size; row++)
                        ((Button)OpponentField.GetControlFromPosition(column, row)).Enabled = false ;
                this.Refresh();
                Thread.Sleep(1000);
                for (int column = 0; column < ModelGame.Size; column++)
                    for (int row = 0; row < ModelGame.Size; row++)
                        ((Button)OpponentField.GetControlFromPosition(column, row)).Enabled = true ;
            }
            if (Game.SecondPlayer.Map.CountDeadCells() == Game.SecondPlayer.Map.AmountOfAliveShips)
            {
                this.Hide();
                var form4 = new Form4();
                form4.Show(this);
            }
        }

        public void UpdatePointOpponent(Point point, State state)
        {
            ((Button)OpponentField.GetControlFromPosition(point.X, point.Y)).BackColor = StateToColor(state);

            if (state != State.Shooting && state != State.KilledShip && state != State.ShootedShip)
            {
                this.Refresh();
                Thread.Sleep(1000);
            }
            if (Game.FirstPlayer.Map.CountDeadCells()== Game.FirstPlayer.Map.AmountOfAliveShips)
            {
                this.Hide();
                var form3 = new Form3();
                form3.Show(this);
            }
        }

        public void FUpdate()
        {
            for (int column = 0; column < ModelGame.Size; column++)
                    for (int row = 0; row < ModelGame.Size; row++)
                    {
                        ((Button)PlayerField.GetControlFromPosition(column, row)).BackColor = StateToColor(Game.FirstPlayer.Map[column, row]);
                        if (Game.SecondPlayer.Map[column, row]==State.Ship)
                            ((Button)OpponentField.GetControlFromPosition(column, row)).BackColor = StateToColor(State.Empty);
                        else
                            ((Button)OpponentField.GetControlFromPosition(column, row)).BackColor = StateToColor(Game.SecondPlayer.Map[column, row]);
                    }
        }

        private static Color StateToColor(State state)
        {
            switch (state)
            {
                case State.Empty:
                    return Color.Aquamarine;
                case State.Ship:
                    return Color.DarkGreen;
                case State.Shooting:
                    return Color.Gray;
                case State.ShootedShip:
                    return Color.Orange;
                case State.KilledShip:
                    return Color.Red;
            }
            return Color.White;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}