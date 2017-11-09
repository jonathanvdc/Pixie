files = \
	*.cs \
	Render/*.cs

libraries = \
	../Pixie/bin/clr/Pixie.dll

ifeq ($(ASM_VERSION),)
	ASM_VERSION:=0.0.1.0
endif

common_options = -asm-name=Pixie.Terminal -asm-version=$(ASM_VERSION) -Wall -Wextra

.PHONY: all flo dll clean
all: flo dll

flo: bin/flo/Pixie.Terminal.flo
dll: bin/clr/Pixie.Terminal.dll

clean:
	rm -rf bin/
	rm -rf obj/

include ../flame-make-scripts/use-ecsc.mk

bin/flo/Pixie.Terminal.flo: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform ir -runtime clr -libs $(libraries) -o $@ $(common_options)

bin/clr/Pixie.Terminal.dll: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform clr -libs $(libraries) -o $@ $(common_options)
