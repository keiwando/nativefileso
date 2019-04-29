mergeInto(LibraryManager.library, {

    /**
     * 
     */
    openFile: function() {
        var inputId = 'nativefileso-webgl-file-input'

        console.log('called in jslib')

        var existingInput = document.getElementById(inputId)
        if (existingInput) {
            document.body.removeChild(existingInput)
        }

        var input = document.createElement('input')
        input.type = 'file'
        input.id = inputId
        input.style.cssText = 'display: none; visibility: hidden;'

        input.accept = '.txt, .evol'

        input.onclick = function() {
            console.log('input clicked')
        }

        input.onchange = function () {

            var file = input.files[0]
            var reader = new FileReader()
            reader.onload = function (e) {
                console.log(e.target.result)
            }
            reader.readAsText(file)
        }

        document.body.appendChild(input)

        setTimeout(function() {
            input.click()
        }, 100)

        // document.onmouseup = function() {
        //     input.click()
        //     document.onmouseup = null
        // }
        
    }
})