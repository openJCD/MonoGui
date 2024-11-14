function Init() 
    w = window(scene_root, "Benchmark Options", 0, 0, 300, 200, "CENTRE", "dialog_benchmark")
    btn_fps = new_plain_button(w, "Show FPS", -20, 35, 100, 50, "TOPRIGHT", "None", "btn_fps")
    btn_create_container = new_plain_button(w, "Create container", 20, 35, 150, 50, "TOPLEFT", "None", "create_container")
    btn_reset = new_plain_button(w, "Reset", -20, -20, 100, 50, "BOTTOMRIGHT", "None", "btn_reset")
    label_container_count = new_text_label(w, "Count: 0", 20, -35, "BOTTOMLEFT")
    
    count = 0
end

function OnButtonClick(sender, e) 
    if sender == btn_fps then
        send_debug_command("fpsMeter")
    end
    if sender == btn_create_container then
        local c = container(scene_root, randint(-400, 400), randint(-300, 300), randint(10, 200), randint(10, 200), "CENTRE")
        count = count + 1
        c.ClipContents=true
        window:Open()
        label_container_count.Text = "Count: " .. count
    end
    if sender == btn_reset then
        load_new_scene(scene_manager, "benchmark.scene")
    end
end
