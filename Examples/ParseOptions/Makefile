files = \
	*.cs

libraries = \
	../../Pixie/bin/clr/Pixie.dll \
	../../Pixie.Terminal/bin/clr/Pixie.Terminal.dll

ifeq ($(ASM_VERSION),)
	ASM_VERSION:=0.0.1.0
endif

common_options = -asm-name=ParseOptions -asm-version=$(ASM_VERSION) -Wall -Wextra

.PHONY: all flo exe clean
all: flo exe

flo: bin/flo/ParseOptions.flo
exe: bin/clr/ParseOptions.exe

clean:
	rm -rf bin/
	rm -rf obj/

include ../../flame-make-scripts/use-ecsc.mk

bin/flo/ParseOptions.flo: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform ir -runtime clr -libs $(libraries) -o $@ $(common_options)

bin/clr/ParseOptions.exe: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform clr -libs $(libraries) -o $@ $(common_options)
