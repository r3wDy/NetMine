using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XtremeSweeper
{

    public enum eGameState
    {
        PLAYING,
        OBSERVING,
        DEAD,
        MENU,
        ENTER_SERVERLOBBY,
        SERVERLOBBY,
        ENTER_CLIENTLOBBY,
        CLIENTLOBBY,
        QUITTING,
        RUNNING
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class cXtremeSweeper_Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        cGameServer m_GameServer;
        cGameClient m_GameClient;
        cMainMenu m_MainMenu;
        cMouseHandler m_TheMouse;
        eGameState m_GameState;

        public cXtremeSweeper_Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_MainMenu = new cMainMenu();
            m_TheMouse = new cMouseHandler(new Vector2(0, 0));
            this.setGameState(eGameState.MENU);
            base.Initialize();
        }


        public eGameState getGameState()
        {
            return m_GameState;
        }

        public void setGameState(eGameState i_NewState)
        {
            this.m_GameState = i_NewState;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            m_TheMouse.setCursor("menuCursor", Content);
            m_MainMenu.loadContent(Content);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            m_TheMouse.Update();

            switch (getGameState())
            {
                case eGameState.MENU:
                    setGameState(m_MainMenu.update());
                    break;

                case eGameState.QUITTING:
                    Exit();
                    break;

                case eGameState.ENTER_SERVERLOBBY:
                    m_GameServer = new cGameServer();
                    m_GameServer.waitForConnection();

                    m_GameClient = new cGameClient();
                    m_GameClient.connect("127.0.0.1");

                    while (!m_GameClient.isConnected());

                    setGameState(eGameState.SERVERLOBBY);
                    break;

                case eGameState.SERVERLOBBY:
                    m_GameClient.sendData(new cInitGameMsg(m_GameClient.getPlayerID(), 15, 15, false, eGAME_TYPE.SOLO));
                    setGameState(eGameState.RUNNING);
                    break;

                case eGameState.RUNNING:

                        m_GameServer.tick(gameTime);
                    break;


            }



            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            // TODO: Add your drawing code here
            Rectangle rec = new Rectangle(0, 0, 640, 480);

            switch( getGameState() )
            {

                case eGameState.MENU:
                    m_MainMenu.renderMainMenu(spriteBatch);
                    break;

                case eGameState.SERVERLOBBY:
                    break;
                case eGameState.RUNNING:
                    break;
            }

            m_TheMouse.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

