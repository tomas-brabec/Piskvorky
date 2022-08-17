using System.Drawing.Drawing2D;
using System.Net.Sockets;

namespace Piskvorky
{
    public partial class MainForm : Form
    {
        private Game game;
        private NetworkConnection connection;

        private float size;
        private float originX;
        private float originY;

        private CancellationTokenSource? cts;

        public MainForm()
        {
            InitializeComponent();
            popupWindow.btnConfirm.Click += btnConfirm_Click;
            popupWindow.btnCancel.Click += btnCancel_Click;
            game = new Game(16);
            connection = new NetworkConnection();
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
            popupWindow.Visible = false;
            cts?.Cancel();
        }

        private void btnConfirm_Click(object? sender, EventArgs e)
        {
            var name = popupWindow.textBoxPrompt.Text.Trim();
            if (string.IsNullOrEmpty(name))
                game.PlayerName = $"Player {Random.Shared.Next(100, 1000)}";
            else
                game.PlayerName = popupWindow.textBoxPrompt.Text;

            labelLeft.Text = game.PlayerName;
            popupWindow.Visible = false;

            EnableNetworkButtons(true);
        }

        private async void btnRunServer_Click(object sender, EventArgs e)
        {
            EnableNetworkButtons(false);
            popupWindow.SetInfoMode("Server čeká na připojení protihráče...");

            cts = new CancellationTokenSource();
            game.PlayerMark = (Player)Random.Shared.Next(1, 3);
            game.CurrentPlayer = Player.O;

            NetworkMessage message = null!;

            popupWindow.Visible = true;

            try
            {
                message = await connection.RunServerAsync(game.PlayerName, game.PlayerMark, cts.Token);
            }
            catch (SocketException ex)
            {
                statusLabel.Text = ex.Message;
            }
            catch (OperationCanceledException ex)
            {
                statusLabel.Text = ex.Message;
                //TODO
            }
            catch (InvalidDataException ex)
            {
                statusLabel.Text = ex.Message;
            }
            finally
            {
                cts.Dispose();
                popupWindow.Visible = false;
            }

            if (message is null)
            {
                EnableNetworkButtons(true);
            }
            else
            {
                labelRight.Text = message.Name;
                game.IsRunning = true;
            }
        }

        private async void btnConnectToServer_Click(object sender, EventArgs e)
        {
            EnableNetworkButtons(false);
            popupWindow.SetInfoMode("Hledám server...");

            cts = new CancellationTokenSource();
            game.CurrentPlayer = Player.O;

            NetworkMessage message = null!;

            popupWindow.Visible = true;

            try
            {
                message = await connection.ConnectToServerAsync(game.PlayerName, cts.Token);
                await Task.Delay(200);
            }
            catch (SocketException ex)
            {
                statusLabel.Text = ex.Message;
            }
            catch (OperationCanceledException ex)
            {
                statusLabel.Text = ex.Message;
                //TODO
            }
            catch (InvalidDataException ex)
            {
                statusLabel.Text = ex.Message;
            }
            finally
            {
                cts.Dispose();
                popupWindow.Visible = false;
            }

            if (message is null)
            {
                EnableNetworkButtons(true);
            }
            else
            {
                game.PlayerMark = message.Mark == Player.X ? Player.O : Player.X;
                labelRight.Text = message.Name;
                game.IsRunning = true;
            }
        }

        private void EnableNetworkButtons(bool enable)
        {
            btnRunServer.Enabled = enable;
            btnConnectToServer.Enabled = enable;
        }
    }
}