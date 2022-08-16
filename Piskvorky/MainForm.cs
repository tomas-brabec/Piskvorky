using System.Drawing.Drawing2D;

namespace Piskvorky
{
    public partial class MainForm : Form
    {
        private Game game;
        private float size;
        private float originX;
        private float originY;

        public MainForm()
        {
            InitializeComponent();
            popupWindow.btnConfirm.Click += btnConfirm_Click;
            popupWindow.btnCancel.Click += btnCancel_Click;
            game = new Game(16);
        }

        private void panelCenter_Resize(object sender, EventArgs e)
        {
            panelCenter.Invalidate();

            if (popupWindow.Visible == true)
                popupWindow.Center();
        }

        private void panelCenter_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.Default;
            graphics.Clear(SystemColors.Control);

            var width = panelCenter.Width;
            var height = panelCenter.Height;
            var margin = 10f;
            var size = Math.Min(width, height) - 2 * margin;
            var originX = margin;
            var originY = margin;

            if (width > height)
                originX = width / 2 - height / 2 + margin;
            else if (height > width)
                originY = height / 2 - width / 2 + margin;

            this.size = size;
            this.originX = originX;
            this.originY = originY;

            var space = size / game.BoardSize;

            var pen = new Pen(Color.Black, 1);


            for (int i = 0; i <= game.BoardSize; i++)
            {
                //draw vertical line
                graphics.DrawLine(pen, originX + i * space,
                    originY,
                    originX + i * space,
                    originY + size);
                //draw horizontal line
                graphics.DrawLine(pen, originX,
                   originY + i * space,
                   originX + size,
                   originY + i * space);
            }

            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var penWidth = space * 0.1f;
            var cellMargin = 2 + penWidth / 2;

            for (int x = 0; x < game.Board.GetLength(0); x++)
            {
                for (int y = 0; y < game.Board.GetLength(1); y++)
                {
                    var cell = game.Board[x, y];

                    if (cell == 0)
                        continue;

                    var cellOriginX = originX + x * space;
                    var cellOriginY = originY + y * space;


                    if (cell == (int)Player.X)
                    {
                        //draw X
                        var bluePen = new Pen(Color.Blue, penWidth);
                        graphics.DrawLine(bluePen,
                            cellOriginX + cellMargin,
                            cellOriginY + cellMargin,
                            cellOriginX + space - cellMargin,
                            cellOriginY + space - cellMargin);
                        graphics.DrawLine(bluePen,
                            cellOriginX + cellMargin,
                            cellOriginY + space - cellMargin,
                            cellOriginX + space - cellMargin,
                            cellOriginY + cellMargin);
                    }
                    else
                    {
                        //draw O
                        graphics.DrawEllipse(new Pen(Color.Red, penWidth),
                            cellOriginX + cellMargin,
                            cellOriginY + cellMargin,
                            space - cellMargin * 2,
                            space - cellMargin * 2);
                    }
                }
            }
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {

        }

        private void btnConfirm_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(popupWindow.textBoxPrompt.Text))
                game.PlayerName = $"Player {Random.Shared.Next(100, 1000)}";
            else
                game.PlayerName = popupWindow.textBoxPrompt.Text;

            labelLeft.Text = game.PlayerName;
            popupWindow.Visible = false;
        }
    }
}