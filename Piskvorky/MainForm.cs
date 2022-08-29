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

        private Color colorX = Color.Red;
        private Color colorO = Color.Blue;

        private CancellationTokenSource? cts;

        public MainForm()
        {
            InitializeComponent();
            popupWindow.btnConfirm.Click += btnConfirm_Click;
            popupWindow.btnCancel.Click += btnCancel_Click;
            game = new Game(16);
            connection = new NetworkConnection();
            connection.OnMessageReceive += Connection_OnMessageReceive;
        }

        private void Connection_OnMessageReceive(object? sender, MessageArgs e)
        {
            game.NextMove(e.NetworkMessage.X, e.NetworkMessage.Y);
            if (game.Winner)
            {
                game.IsRunning = false;
                connection.Close();
                var winner = game.CurrentPlayer == game.PlayerMark ? labelLeft.Text : labelRight.Text;
                popupWindow.SetInfoMode($"Vyhrává hráč {winner}");
                popupWindow.Visible = true;
            }
            else
            {
                game.CurrentPlayer = game.CurrentPlayer == Player.X ? Player.O : Player.X;
                RedrawMarks();
            }

            panelCenter.Invalidate();
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
                        var bluePen = new Pen(colorX, penWidth);
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
                        graphics.DrawEllipse(new Pen(colorO, penWidth),
                            cellOriginX + cellMargin,
                            cellOriginY + cellMargin,
                            space - cellMargin * 2,
                            space - cellMargin * 2);
                    }
                }
            }

            //draw winning row
            if (game.Winner)
            {
                var start = game.First;
                var end = game.Last;

                graphics.DrawLine(new Pen(Color.Gray, penWidth * 2),
                    originX + start.x * space + space / 2,
                    originY + start.y * space + space / 2,
                    originX + end.x * space + space / 2,
                    originY + end.y * space + space / 2);
            }
        }

        private async void panelCenter_Click(object sender, EventArgs e)
        {
            if (!game.IsRunning || game.PlayerMark != game.CurrentPlayer)
                return;

            var args = e as MouseEventArgs;

            if (args is null)
                return;

            var x = args.X;
            var y = args.Y;

            if (x < originX || x > originX + size || y < originY || y > originY + size)
                return;

            var coordinates = ConvertToBoardCoordinates(x, y);
            if (game.NextMove(coordinates.x, coordinates.y))
            {
                if (game.Winner)
                {
                    game.IsRunning = false;
                    var winner = game.CurrentPlayer == game.PlayerMark ? labelLeft.Text : labelRight.Text;
                    popupWindow.SetInfoMode($"Vyhrává hráč {winner}");
                    popupWindow.Visible = true;
                }
                else
                {
                    game.CurrentPlayer = game.CurrentPlayer == Player.X ? Player.O : Player.X;
                    RedrawMarks();
                }

                panelCenter.Invalidate();

                try
                {
                    Console.WriteLine("Test");
                    await connection.SendMessageAsync(new NetworkMessage() { X = coordinates.x, Y = coordinates.y });
                    if (!game.Winner)
                        await connection.ReceiveMessageAsync();
                }
                catch (TaskCanceledException ex)
                {
                    statusLabel.Text = ex.Message;
                }
                catch (IOException ex)
                {
                    game.Reset();
                    panelCenter.Invalidate();
                    EnableNetworkButtons(true);
                    statusLabel.Text = ex.Message;
                }
                catch (Exception ex)
                {
                    statusLabel.Text = ex.Message;
                }
                finally
                {
                    if (game.Winner)
                    {
                        connection.Close();
                        EnableNetworkButtons(true);
                    }
                }
            }
        }
        private (int x, int y) ConvertToBoardCoordinates(int x, int y)
        {
            var cx = (x - originX) / (size / game.BoardSize);
            var cy = (y - originY) / (size / game.BoardSize);

            return ((int)cx, (int)cy);
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            cts?.Cancel();
            popupWindow.Visible = false;
        }

        private void btnConfirm_Click(object? sender, EventArgs e)
        {
            var name = popupWindow.textBoxPrompt.Text.Trim();
            if (string.IsNullOrEmpty(name))
                game.PlayerName = $"Player {Random.Shared.Next(100, 1000)}";
            else
                game.PlayerName = popupWindow.textBoxPrompt.Text;

            this.Text = $"Piškvorky - {game.PlayerName}";
            labelLeft.Text = game.PlayerName;
            popupWindow.Visible = false;

            EnableNetworkButtons(true);
        }

        private async void btnRunServer_Click(object sender, EventArgs e)
        {
            if (game.Winner)
            {
                game.Reset();
                panelCenter.Invalidate();
            }

            statusLabel.Text = "";
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
            }
            catch (InvalidDataException ex)
            {
                statusLabel.Text = ex.Message;
            }
            finally
            {
                cts.Dispose();
                cts = null;
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
                RedrawMarks();

                if (game.PlayerMark != game.CurrentPlayer)
                {
                    try
                    {
                        await connection.ReceiveMessageAsync();
                    }
                    catch (IOException ex)
                    {
                        game.Reset();
                        panelCenter.Invalidate();
                        EnableNetworkButtons(true);
                        statusLabel.Text = ex.Message;
                    }
                }
            }
        }

        private async void btnConnectToServer_Click(object sender, EventArgs e)
        {
            if (game.Winner)
            {
                game.Reset();
                panelCenter.Invalidate();
            }

            statusLabel.Text = "";
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
                throw;
            }
            catch (OperationCanceledException ex)
            {
                statusLabel.Text = ex.Message;
            }
            catch (InvalidDataException ex)
            {
                statusLabel.Text = ex.Message;
            }
            finally
            {
                cts.Dispose();
                cts = null;
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
                RedrawMarks();

                if (game.PlayerMark != game.CurrentPlayer)
                {
                    try
                    {
                        await connection.ReceiveMessageAsync();
                    }
                    catch (IOException ex)
                    {
                        game.Reset();
                        panelCenter.Invalidate();
                        EnableNetworkButtons(true);
                        statusLabel.Text = ex.Message;
                    }
                }
            }
        }

        private void EnableNetworkButtons(bool enable)
        {
            btnRunServer.Enabled = enable;
            btnConnectToServer.Enabled = enable;
        }

        private void panelLeft_Paint(object sender, PaintEventArgs e)
        {
            DrawPlayerMark(sender as Control, game.PlayerMark, e.Graphics);
        }

        private void panelRight_Paint(object sender, PaintEventArgs e)
        {
            DrawPlayerMark(sender as Control, game.OpponentMark, e.Graphics);
        }

        private void RedrawMarks()
        {
            panelLeft.Invalidate();
            panelRight.Invalidate();
        }

        private void DrawPlayerMark(Control? sender, Player playerMark, Graphics graphics)
        {
            if (sender is null)
                return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(SystemColors.Control);

            if (game.IsRunning)
            {
                var size = 40f;
                var penWidth = 8;
                var originX = sender.Width / 2 - size / 2;
                var originY = 40;
                var isCurrent = game.CurrentPlayer == playerMark;

                if (playerMark == Player.X)
                {
                    var bluePen = new Pen(isCurrent ? colorX : Color.Gray, penWidth);
                    graphics.DrawLine(bluePen,
                        originX,
                        originY,
                        originX + size,
                        originY + size);
                    graphics.DrawLine(bluePen,
                        originX,
                        originY + size,
                        originX + size,
                        originY);
                }
                else
                {
                    graphics.DrawEllipse(new Pen(isCurrent ? colorO : Color.Gray, penWidth),
                            originX,
                            originY,
                            size,
                            size);
                }
            }
        }
    }
}