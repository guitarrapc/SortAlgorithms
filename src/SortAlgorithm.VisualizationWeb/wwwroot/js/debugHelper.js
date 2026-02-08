// デバッグヘルパー - C#とJavaScriptのデバッグログを統一管理

window.debugHelper = {
    isEnabled: false,
    
    /**
     * デバッグモードを設定（C#から呼び出される）
     * @param {boolean} enabled - デバッグモードを有効にするか
     */
    setDebugMode: function(enabled) {
        this.isEnabled = enabled;
        if (enabled) {
            console.log('[DebugHelper] Debug mode enabled');
        }
    },
    
    /**
     * 条件付きログ出力
     * @param {...any} args - ログ引数
     */
    log: function(...args) {
        if (this.isEnabled) {
            console.log(...args);
        }
    },
    
    /**
     * 条件付きエラーログ出力
     * @param {...any} args - ログ引数
     */
    error: function(...args) {
        if (this.isEnabled) {
            console.error(...args);
        }
    },
    
    /**
     * 条件付き警告ログ出力
     * @param {...any} args - ログ引数
     */
    warn: function(...args) {
        if (this.isEnabled) {
            console.warn(...args);
        }
    }
};
