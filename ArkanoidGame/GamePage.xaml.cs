using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArkanoidGame
{
    public partial class GamePage : Page
    {
        string maincolor = "violet"; // основной цвет кнопок
        bool playerGoLeft = false;
        bool playerGoRight = false;
        bool gamePlay = true;

        public int levelnum = 1; // счетчик уровеней, всего 10, 10ый босс
        public int allPoints = 0;
        public int pointsLeft = 0;
        public int points = 0;

        public int hearts = 3; // количество жизней
        public bool reloadedShoot = false;
        public bool stickyPlayer = false;

        // для босса
        public bool betterHit = false;
        public int changeOrientation = 0;
        public bool UnChangeOrientation = false;
        public List<int> headsDirections = new List<int>();
        DispatcherTimer changeHeadsDirectionsTimer = new DispatcherTimer();

        public Brick[,] bricks = new Brick[13, 21];
        public int numberOfBricksLeft = 0;

        const  int tickRate = 10;
        Physics Physics = new Physics(tickRate);
        CartesianPosition CurrentPosition;

        DispatcherTimer gameTimer = new DispatcherTimer();
        DispatcherTimer BossHitTimer = new DispatcherTimer();
        DispatcherTimer reloadingShoot = new DispatcherTimer();
        Booster booster = new Booster();
        List<Ball> balls = new List<Ball>();

        // размеры игрового поля
        int height;
        int width;

        public GamePage() // запуск начального уровня
        {
            InitializeComponent();
            bricks = Tools.ReadLvl(levelnum);
            levelTB.Text = "Level " + levelnum;
            Game();
        }
        public GamePage(int level, int allpkt) // запуск следующего уровня
        {
            InitializeComponent();
            levelnum = level;
            allPoints = allpkt;
            levelTB.Text = "Level " + levelnum;
            points = 0;
            bricks = Tools.ReadLvl(levelnum);
            Game();
        }
        public GamePage(int allpkt) // запуск босс уровня
        {
            InitializeComponent();
            levelnum = 10;
            levelTB.Text = "Level BOSS";
            powerIcon.Visibility = Visibility.Visible;
            Game();
            BossLevel();
        }
        private void Game() // запуск самой игры
        {
            height = (int)myCanvas.Height;
            width = (int)myCanvas.Width;

            foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(x => x.Tag.ToString() == "ballEclipse"))
            {
                Ball ball = new Ball();
                ball.InitBall(x, 0);
                balls.Add(ball);
            }

            if (levelnum != 10 && levelnum != 0)
            {
                numberOfBricksLeft = Tools.NumberOfBricks;
                Brick.GenerateElements(ref myCanvas, ref bricks, width, height);
                myCanvas.Focus();
            }

            pointsLabel.Content = "" + allPoints;
            heartsTextBlock.Text = "" + hearts;
            pointsLeft = Tools.PointsAtLevel;
            numberOfBricksLeft = Tools.NumberOfBricks;

            if (levelnum == 10)
            {
                BossLevel();
                Tools.PointsAtLevel = 3500;
            }

            // game cicle
            gameTimer.Interval = TimeSpan.FromMilliseconds(30);
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();
            reloadingShoot.Interval = TimeSpan.FromMilliseconds(3000);
            reloadingShoot.Tick += new EventHandler(Shooting);
        }

        private void BossLevel() // босс уровень
        {
            Tools.SpawnBoss(ref myCanvas); // добавили босса
            for (int j = 0; j < myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").Count(); j++)
            {
                int randomNumber = Tools.RundomNumber(1, 4);
                headsDirections.Add(randomNumber);
            }

            // направление движения голов босса
            changeHeadsDirectionsTimer.Interval = TimeSpan.FromMilliseconds(300);
            changeHeadsDirectionsTimer.Tick += new EventHandler(ChangeHeadsDirection);

            // выстрелы босса
            BossHitTimer.Interval = TimeSpan.FromMilliseconds(5000);
            BossHitTimer.Tick += new EventHandler(BossHit);

            BossHitTimer.Start();
            changeHeadsDirectionsTimer.Start();

            myCanvas.Focus();
        }
        private void ChangeHeadsDirection(object sender, EventArgs e) // изменение движения голов босса
        {
            headsDirections.Clear();
            for (int j = 0; j < myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").Count(); j++)
            {
                int randomNumber = Tools.RundomNumber(1, 4);
                headsDirections.Add(randomNumber);
            }
        }
        private void BossHit(object sender, EventArgs e) // выстрел босса
        {
            if (betterHit)
            {
                Tools.SpawnBossHead(ref myCanvas, ref headsDirections, UnChangeOrientation);
                Tools.SpawnShoots(ref myCanvas, ref balls, player, true);
                betterHit = false;
                changeOrientation++;
            }
            else
            {
                Tools.SpawnShoots(ref myCanvas, ref balls, player, true);
                betterHit = true;
                changeOrientation++;
            }
            if (changeOrientation > 5 && !UnChangeOrientation)
            {
                RotateCanvas();
                UnChangeOrientation = true;
                changeOrientation = 0;
            }
            else if (changeOrientation > 5 && UnChangeOrientation)
            {
                UnRotateCanvas();
                UnChangeOrientation = false;
                changeOrientation = 0;
            }
        }
        private void RotateCanvas() // перевернуть игровое поле на 180 градусов относительно центра
        {
            RotateTransform rotateTransform = new RotateTransform(180);
            rotateTransform.CenterX = 396;
            rotateTransform.CenterY = 413;
            myCanvas.RenderTransform = rotateTransform;

            rotateTransform = new RotateTransform(180);
            rotateTransform.CenterX = 100;
            rotateTransform.CenterY = 150;
            foreach (var x in myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "boss"))
            {
                x.RenderTransform = rotateTransform;
            }

            rotateTransform = new RotateTransform(180);
            rotateTransform.CenterX = 25;
            rotateTransform.CenterY = 38;
            foreach (var x in myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads"))
            {
                x.RenderTransform = rotateTransform;
            }
        }
        private void UnRotateCanvas() // вернуть поле в исходное положение
        {
            RotateTransform rotateTransform = new RotateTransform(0);
            rotateTransform.CenterX = 396;
            rotateTransform.CenterY = 413;
            myCanvas.RenderTransform = rotateTransform;


            rotateTransform = new RotateTransform(0);
            rotateTransform.CenterX = 100;
            rotateTransform.CenterY = 150;
            foreach (var x in myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "boss"))
            {
                x.RenderTransform = rotateTransform;
            }

            rotateTransform = new RotateTransform(0);
            rotateTransform.CenterX = 25;
            rotateTransform.CenterY = 38;
            foreach (var x in myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads"))
            {
                x.RenderTransform = rotateTransform;
            }
        }

        public void RespawnBoost(int indexOfBall) // с вероятностью 10% выпадет бустер
        {
            if (Tools.RundomNumber(1, 10) == 5)
            {
                if (myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster").Count() == 0)
                    booster = new Booster(balls[indexOfBall], ref myCanvas, booster);
            }
        }
        public void Next_Level() // запуск следующего уровня
        {
            gameTimer.Stop();
            levelnum++;

            if (levelnum == 10) // Boss
                NavigationService.Navigate(new GamePage(allPoints));
            else
                NavigationService.Navigate(new GamePage(levelnum, allPoints));
        }
        public void SetBoost() // активировать бустер
        {
            if (booster.GetPower() != Power.None)
                powerIcon.Visibility = Visibility.Visible;

            switch (booster.GetPower())
            {
                case Power.PlayerLenght:
                    booster.SetBoostPlayerLenght(ref player);
                    powerTextBlock.Text = "Player lenght";
                    powerIcon.Source = new BitmapImage(new Uri("/Images/boost-player-length.png", UriKind.Relative));
                    break;
                case Power.NewBall:
                    booster.NewBallSetBoost(ref myCanvas, ref balls);
                    powerTextBlock.Text = "New ball";
                    powerIcon.Source = new BitmapImage(new Uri("/Images/boost-add-ball.png", UriKind.Relative));
                    break;
                case Power.StrongerHit:
                    booster.SetPower(Power.StrongerHit);
                    powerTextBlock.Text = "Stronger hit";
                    powerIcon.Source = new BitmapImage(new Uri("/Images/boost-stronger-hit.png", UriKind.Relative));
                    break;
                case Power.Shooting:
                    powerTextBlock.Text = "Shooting";
                    powerIcon.Source = new BitmapImage(new Uri("/Images/boost-shooting-mode.png", UriKind.Relative));
                    reloadingShoot.Start();
                    break;
                case Power.StickyPlayer:
                    powerTextBlock.Text = "Sticky player";
                    powerIcon.Source = new BitmapImage(new Uri("/Images/boost-sticky-player.png", UriKind.Relative));
                    stickyPlayer = true;
                    break;
                case Power.None:
                    powerTextBlock.Text = "";
                    break;
                default:
                    break;
            }
        }
        public void StopBoost() // отключить бустер
        {
            powerIcon.Visibility = Visibility.Collapsed;

            if (booster.GetPower() != Power.None)
                powerIcon.Visibility = Visibility.Visible;

            switch (booster.GetPower())
            {
                case Power.PlayerLenght:
                    booster.StopBoostPlayerLenght(ref player);
                    break;
                case Power.NewBall:
                    break;
                case Power.StrongerHit:
                    booster.SetPower(Power.None);
                    break;
                case Power.Shooting:
                    reloadedShoot = false;
                    reloadingShoot.Stop();
                    break;
                case Power.StickyPlayer:
                    stickyPlayer = false;
                    break;
                case Power.None:
                    break;
                default:
                    break;
            }
        }
        public bool PlayerCaughtABoost(Rect rect) // если платформа коснулась бустера, то активируем его
        {
            foreach (var g in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster"))
            {
                Rect boosterEclipseHitBox = new Rect(Canvas.GetLeft(g), Canvas.GetTop(g), g.Width, g.Height);
                if (boosterEclipseHitBox.IntersectsWith(rect))
                {
                    myCanvas.Children.Remove(g);
                    StopBoost();
                    booster.RandomPower();
                    SetBoost();
                    return true;
                }
            }
            return false;
        }
        private bool ChangeBallDirection(int index) // направление шарика
        {
            if (balls[index].posY <= 0)
            {
                if (balls[index].iAmShoot)
                {
                    balls.RemoveAt(index);
                    return false;
                }
                balls[index].top = false;
            }
            if (balls[index].posX <= 0) balls[index].left = false; // лево
            if (balls[index].posX >= width - (balls[index].rad * 2)) balls[index].left = true;// право
            return true;
        }
        public void HitBlock(int posX, int posY, Rectangle rectangle, int indexOfBall) // действия при касании блока
        {
            if (rectangle.Tag.ToString() == "bossHeads")
            {
                myCanvas.Children.Remove(rectangle);
                headsDirections.RemoveAt(headsDirections.Count - 1);
                return;
            }
            if (bricks[posX, posY].GetType() != typeof(GoldBrick))
            {
                if (bricks[posX, posY].TimesToBreak < 2)
                {
                    myCanvas.Children.Remove(rectangle);

                    RespawnBoost(indexOfBall);

                    points += bricks[posX, posY].Value;
                    allPoints += bricks[posX, posY].Value;
                    pointsLabel.Content = "" + allPoints;

                    if (points == Tools.PointsAtLevel) Next_Level();
                }
                else
                {
                    bricks[posX, posY].TimesToBreak--;
                    return;
                }
            }
        }
        private bool BallMovement(int index, int goLeft = 0) // передвижение шарика
        {
            
            balls[index].posX = Canvas.GetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
            balls[index].posY = Canvas.GetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
            CurrentPosition = Physics.ExtractValue(balls[index].left, balls[index].top, balls[index].position);

            if (balls[index].stop != true)
            {
                balls[index].posX += CurrentPosition.HorizontalPosition;
                balls[index].posY += CurrentPosition.VerticalPosition;
            }
            else 
                balls[index].posX += goLeft;

            Canvas.SetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posX);
            Canvas.SetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posY);

            if (!ChangeBallDirection(index))
            {
                myCanvas.Children.Remove(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
                return false;
            }
            return true;
        }
        private void PlayerMovement(bool direction, int playerSpeed = 2) // передвижение игрока
        {
            for (int i = 0; i < playerSpeed; i++)
            {
                if (Canvas.GetLeft(player) + (player.Width) < width && direction)
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) + 1);
                    for (int j = 0; j < balls.Count; j++)
                    {
                        if (balls[j].stop == true)
                            BallMovement(j, 1);
                    }
                }
                if (Canvas.GetLeft(player) > 0 && !direction)
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) - 1);
                    for (int j = 0; j < balls.Count; j++)
                    {
                        if (balls[j].stop == true)
                            BallMovement(j, -1);
                    }
                }
            }
        }
        private void BossHeadsMovement(int index) // передвижение голов босса
        {
            double posX = Canvas.GetLeft(myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").ElementAt(index));
            double posY = Canvas.GetTop(myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").ElementAt(index));



            if (posX < 15 || posX + 50 > 780 || posY > 600 || posY < 15)
            {
                if (posX > 400 && posY > 400)
                    headsDirections[index] = 4;
                if (posX < 400 && posY > 400)
                    headsDirections[index] = 3;
                if (posX > 400 && posY < 400)
                    headsDirections[index] = 2;
                if (posX < 400 && posY < 400)
                    headsDirections[index] = 1;
            }

            if (headsDirections.Count == 0)
            {
                return;
            }

            switch (headsDirections[index])
            {
                case 1:
                    posX += 0.5;
                    posY += 0.5;
                    break;
                case 2:
                    posX += -0.5;
                    posY += 0.5;
                    break;
                case 3:
                    posX += 0.5;
                    posY += -0.5;
                    break;
                case 4:
                    posX += -0.5;
                    posY += -0.5;
                    break;
            }

            Canvas.SetLeft(myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").ElementAt(index), posX);
            Canvas.SetTop(myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").ElementAt(index), posY);
        }
        private void BoostMovement() // передвижение бустера
        {
            foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster"))
            {
                booster.posX = (int)Canvas.GetLeft(x);
                booster.posY = (int)Canvas.GetTop(x);

                booster.posY += 1;

                Canvas.SetLeft(x, booster.posX);
                Canvas.SetTop(x, booster.posY);
            }
        }
        private void GameTimerEvent(object sender, EventArgs e)
        {
            try
            {
                OnLoseAllBalls();
  
                for (int i = 0; i < tickRate; i++)
                {
                    bool isTheSameBrick = false;
                    foreach (var x in myCanvas.Children.OfType<Rectangle>()) // столкновение с блоком
                    {
                        bool leave = false;
                        if (!isTheSameBrick && x.Name != "player" && x.Name != "boss") //если элемент блок
                        {
                            int posX = (int)Canvas.GetLeft(x) / (width / 13); 
                            int posY = (int)Canvas.GetTop(x) / (height / 26); 
                            Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect ballEclipseHitBox;

                            foreach (var (ball, index) in myCanvas.Children.OfType<Ellipse>().Where(ball => ball.Tag.ToString() == "ballEclipse").Select((ball, index) => (ball, index)))
                            {
                                leave = false;
                                ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);

                                if (!isTheSameBrick && ballEclipseHitBox.IntersectsWith(BlockHitBox))
                                {
                                    // верх блока
                                    if (balls[index].posY + balls[index].rad < Canvas.GetTop(x))
                                    {
                                        if (booster.GetPower() != Power.StrongerHit && levelnum ==  10)
                                        {
                                            balls[index].top = true;
                                        }
                                        else if (levelnum != 0 && levelnum != 10)
                                        {
                                            if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                            {
                                                balls[index].top = true;
                                            }
                                        }

                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }

                                    // низ блока
                                    else if (balls[index].posY + balls[index].rad > Canvas.GetTop(x) + x.Height)
                                    {
                                        if (booster.GetPower() != Power.StrongerHit && levelnum == 10)
                                        {
                                            balls[index].top = false;
                                        }
                                        else if (levelnum != 0 && levelnum != 10)
                                        {
                                            if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                            {
                                                balls[index].top = false;
                                            }
                                        }

                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }

                                    // лево блока
                                    else if (balls[index].posX + balls[index].rad < Canvas.GetLeft(x))
                                    {
                                        if (booster.GetPower() != Power.StrongerHit && levelnum == 10)
                                        {
                                            balls[index].left = true;
                                        }
                                        else if (levelnum != 0 && levelnum != 10)
                                        {
                                            if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                            {
                                                balls[index].left = true;
                                            }
                                        }

                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }

                                    // право блока
                                    else if (balls[index].posX + balls[index].rad > Canvas.GetLeft(x) + x.Width)
                                    {
                                        if (booster.GetPower() != Power.StrongerHit && levelnum == 10)
                                        {
                                            balls[index].left = false;
                                        }
                                        else if (levelnum != 0 && levelnum != 10)
                                        {
                                            if (booster.GetPower() != Power.StrongerHit || bricks[posX, posY].GetType() == typeof(GoldBrick))
                                            {
                                                balls[index].left = false;
                                            }
                                        }

                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }
                                    if (balls[index].iAmShoot)
                                    {
                                        balls.RemoveAt(index);
                                        myCanvas.Children.Remove(ball);
                                        leave = true;
                                    }
                                }
                                if (leave || isTheSameBrick)
                                    break;
                            }
                            if (leave)
                                break;
                        }
                        if (x.Name == "player") // если игрок, то отскок
                        {
                            Rect blockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect ballEclipseHitBox;

                            foreach (var (ball, index) in myCanvas.Children.OfType<Ellipse>().Where(ball => ball.Tag.ToString() == "ballEclipse").Select((ball, index) => (ball, index)))
                            {
                                bool gotHit = false;
                                ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                                gotHit = Tools.CalculateTrajectory(blockHitBox, ballEclipseHitBox, x, ball, ref balls, index, stickyPlayer, tickRate);
                                if (!gotHit)
                                {
                                    for (int b = 0; b < myCanvas.Children.OfType<Ellipse>().Where(deletedBall => deletedBall.Tag.ToString() == "ballEclipse").Count(); b++)
                                    {
                                        balls.RemoveAt(b);
                                        myCanvas.Children.Remove(myCanvas.Children.OfType<Ellipse>().Where(deletedBall => deletedBall.Tag.ToString() == "ballEclipse").ElementAt(b));
                                    }
                                    return;
                                }
                            }
                            
                            if (PlayerCaughtABoost(blockHitBox)) { break; }
                        }
                        if (x.Name == "boss")
                        {
                            Rect blockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect ballEclipseHitBox;
                            foreach (var (ball, index) in myCanvas.Children.OfType<Ellipse>().Where(ball => ball.Tag.ToString() == "ballEclipse").Select((ball, index) => (ball, index)))
                            {
                                if (balls[index].iAmBossShoot) break;
                                ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                                if (ballEclipseHitBox.IntersectsWith(blockHitBox))
                                {
                                    if (booster.GetPower() == Power.StrongerHit)
                                    {
                                        points += 210;
                                        allPoints += 210;
                                    }
                                    else
                                    {
                                        points += 70;
                                        allPoints += 70;
                                    }
                                    pointsLabel.Content = "" + allPoints;

                                    if (points >= Tools.PointsAtLevel)
                                    {
                                        MessageBox.Show("Win :)");
                                        gameTimer.Stop();
                                        NavigationService.Navigate(new MenuPage());
                                    }
                                    RespawnBoost(index);
                                    // верх блока
                                    if (balls[index].posY + balls[index].rad < Canvas.GetTop(x))
                                    {
                                        balls[index].top = true;
                                        leave = true;
                                    }

                                    // низ блока
                                    else if (balls[index].posY + balls[index].rad > Canvas.GetTop(x) + x.Height)
                                    {
                                        balls[index].top = false;
                                        leave = true;
                                    }

                                    // лево блока
                                    else if (balls[index].posX + balls[index].rad < Canvas.GetLeft(x))
                                    {
                                        balls[index].left = true;
                                        leave = true;
                                    }

                                    // право блока
                                    else if (balls[index].posX + balls[index].rad > Canvas.GetLeft(x) + x.Width)
                                    {
                                        balls[index].left = false;
                                        leave = true;
                                    }
                                    if (balls[index].iAmShoot)
                                    {
                                        balls.RemoveAt(index);
                                        myCanvas.Children.Remove(ball);
                                        leave = true;
                                    }
                                }
                                if (leave)
                                    break;
                            }
                        }
                        if (leave)
                            break;
                    }

                        if (playerGoRight && !playerGoLeft)
                            PlayerMovement(true);
                        if (playerGoLeft && !playerGoRight)
                            PlayerMovement(false);


                    for (int j = 0; j < myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").Count(); j++)
                    {
                        if (!BallMovement(j))
                        {
                            break;
                        }
                    }

                    for (int j = 0; j < myCanvas.Children.OfType<Rectangle>().Where(element => element.Tag.ToString() == "bossHeads").Count(); j++)
                    {
                        BossHeadsMovement(j);
                    }

                    BoostMovement();

                    foreach (var (element, index) in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").Select((element, index) => (element, index)))
                    {
                        if (Canvas.GetTop(element) > Canvas.GetTop(player))
                        {
                            myCanvas.Children.Remove(element);
                            balls.RemoveAt(index);
                            if (balls.Count == 0) return;
                            break;
                        }
                    }

                    foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster"))
                    {
                        if (Canvas.GetTop(x) > Canvas.GetTop(player))
                        {
                            myCanvas.Children.Remove(x);
                            break;
                        }
                    }
                }
                if (Canvas.GetLeft(player) >= width) Next_Level();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "" + ex.StackTrace);
            }

        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }
        private void myCanvas_KeyDown(object sender, KeyEventArgs e) // при зажатой кнопке двигаемся
        {
            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = true;
                    break;
                case Key.Right:
                    e.Handled = true;
                    break;
                case Key.Up:
                    e.Handled = true;
                    break;
                case Key.Down:
                    e.Handled = true;
                    break;
                case Key.Tab:
                    e.Handled = true;
                    break;
                default:
                    break;
            }

            if (e.Key == Key.D) playerGoRight = true;

            if (e.Key == Key.A) playerGoLeft = true;

            if (e.Key == Key.Space)
                for (int i = 0; i < balls.Count; i++)
                    balls[i].stop = false;
        }
        private void myCanvas_KeyUp(object sender, KeyEventArgs e) // при разжатой нет
        {
            if (e.Key == Key.D) playerGoRight = false;
            if (e.Key == Key.A) playerGoLeft = false;
        }
        private void Button_MouseEvent(object sender, MouseEventArgs e) // при наведении на кнопку меняем ее на картинку другого цвета
        {
            (Application.Current.MainWindow as MainWindow).changeColors(sender as Button, maincolor);
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Image img = (Image)btn.Content;

            if (gamePlay)
            {
                gameTimer.Stop();
                gamePlay = false;
                myCanvas.Focus();
                img.Source = new BitmapImage(new Uri("Images/play-white.png", UriKind.Relative));
            }
            else
            {
                gameTimer.Start();
                gamePlay = true;
                myCanvas.Focus();
                img.Source = new BitmapImage(new Uri("Images/pause-white.png", UriKind.Relative));
            }
        }
        private void Shooting(object sender, EventArgs e) // когда бустер шутинг
        {
            reloadedShoot = true;
        }
        public void OnLoseAllBalls() // при потере шарика
        {
            heartsTextBlock.Text = "" + hearts;

            if (balls.Count == 0 && hearts == 0)
            {
                MessageBox.Show("Lose :(");
                gameTimer.Stop();
                NavigationService.Navigate(new MenuPage());
                return;
            }
            else if (balls.Count == 0 && hearts > 0)
            {
                Tools.SpawnBall(ref myCanvas, ref balls, player);
                hearts--;
            }
        }
    }
}
