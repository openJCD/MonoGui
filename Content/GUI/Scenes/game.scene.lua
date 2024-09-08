require "api_common"
function Init()
    DISPLAY_W = game_graphics.PreferredBackBufferWidth;
    DISPLAY_H = game_graphics.PreferredBackBufferHeight;

    tx_world = texture_from_file(game_graphics.GraphicsDevice, "Content/Game/WORLDMAP.png")
    tx_btn_benchmark = texture_from_file(game_graphics.GraphicsDevice, "Content/GUI/Textures/Button/icon_benchmark.png")
    tx_btn_terminal = texture_from_file(game_graphics.GraphicsDevice, "Content/GUI/Textures/Button/icon_terminal.png")
    send_debug_message("creating camera + draw layers...")
    world_camera = new_camera()
    world_camera.ViewportWidth = game_graphics.PreferredBackBufferWidth 
    world_camera.ViewportHeight = game_graphics.PreferredBackBufferHeight
    world_camera:CreateCamTarget("left_target", tx_world.Bounds.Width/4, tx_world.Bounds.Height/2, 2.0)
    world_camera:CreateCamTarget("world_target", tx_world.Bounds.Width/2, tx_world.Bounds.Height/2, 2.0)
    world_camera:CreateCamTarget("right_target", tx_world.Bounds.Width*0.75, tx_world.Bounds.Height/2, 2.0)
    world_camera:SetZoomClamp(1, 10):SetMoveSpeed(100)
    world_layer = new_draw_layer(game_graphics.GraphicsDevice, world_camera)
    send_debug_message("done.")

    send_debug_message("creating gui...")
    container_taskbar = container(scene_root, 0, 0, scene_root.Width-1, 30, anchor.top_left)
    container_taskbar.RenderBackgroundColor = false;
    
    btn_apps = new_plain_button(container_taskbar, "//Apps", -55, 0, 100, 25, "CENTRE", "OpenWindow", "dialog_apps")
    dialog_apps = container(scene_root, 0, 0, 300, 350, "BOTTOMLEFT")
    dialog_apps:Close()
    dialog_apps.RenderBackgroundColor = true;   
    btn_app_load_benchmark = new_icon_button(dialog_apps, tx_btn_benchmark, 10, 10, anchor.top_left, btnEvent.none, "btn_benchmark") 
    btn_app_terminal = new_icon_button(dialog_apps, tx_btn_terminal, 84, 10, anchor.top_left, btnEvent.none, "terminal")
    --container_bl = new_root_container(scene_root, 0,0, 300, 50, "BOTTOMLEFT");
    --tx1 = new_text_input(container_bl);

    btn_options = new_plain_button(container_taskbar, "//Options", 55, 0, 100, 25, anchor.center, btnEvent.open_target, "dialog_options")
    dialog_options = create_standard_options_dialog(scene_root,  "dialog_options") 
    send_debug_message("done.")
end 

function OnButtonClick (sender, e) 
    options_dialog_OnButtonClick(sender, e)
    if sender == btn_app_load_benchmark then
        load_new_scene(scene_manager, "benchmark.scene")
    end
    if sender == btn_apps then 
        if not (dialog_apps.IsOpen) then 
            sequenced_custom({dialog_apps}, 0, 350, 1, ease.outCubic)
        end
        dialog_apps.IsOpen = not dialog_apps.IsOpen
    end
end

function OnUIUpdate()
    container_taskbar.Width = scene_root.Width 
    options_dialog_txt_zoomlevel.Text = "Zoom:".. round(world_camera.Zoom, 2)
end

function OnGameUpdate(gameTime)
    world_camera:Update(gameTime)
end

function OnGameDraw()   
    world_camera.ViewportWidth = game_graphics.PreferredBackBufferWidth 
    world_camera.ViewportHeight = game_graphics.PreferredBackBufferHeight
    world_layer:BeginDraw()
    world_layer:DrawTexture(tx_world, tx_world.Bounds, nil)
    world_layer:EndDraw()
end

function OnKeyPressed (sender, e) 
    --send_debug_message(e.first_key_as_string)
    if e.first_key_as_string == "Right" then
        -- go to next target in list
        world_camera:GoToNextTarget() 
    end
    if e.first_key_as_string== "Left" then
        -- go to previous target in list.
        world_camera:GoToPrevTarget() 
    end
    if e.first_key_as_string == "Up" then
        -- zoom in
        world_camera:IncrementZoom(1)
    end
    if e.first_key_as_string == "Down" then
        -- zoom out
        world_camera:IncrementZoom(-1)
    end
    if e.first_key_as_string == "R" then
        -- Reset Zoom
        world_camera.ActiveTarget:ResetZoom()
    end
end
